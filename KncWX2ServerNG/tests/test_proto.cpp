// ---------------------------------------------------------------------------
// Conformance tests for the packet envelope: SerBuffer, SecureBuffer/SA,
// framing, and KEvent.
// ---------------------------------------------------------------------------
#include "test_util.hpp"

#include "../src/proto/event.cppm"
#include "../src/proto/framing.cppm"
#include "../src/proto/security.cppm"

using namespace knc;
using namespace knc::proto;
using knctest::from_ascii;
using knctest::from_hex;
using knctest::to_hex;

namespace {

Bytes iv_of(std::uint8_t b) { return Bytes(kIvSize, b); }

// ===========================================================================
// SerBuffer
// ===========================================================================

void test_serbuffer(knctest::Suite& s) {
    s.run("empty SerBuffer is a bare zero length", [] {
        SerBuffer buf;
        Writer w;
        buf.write_to(w);
        // No compressed flag and no body when the length is zero.
        KNC_CHECK_EQ_BYTES(std::move(w).take(), from_hex("00000000"),
                           "empty SerBuffer layout");
    });

    s.run("non-empty SerBuffer = BE length + flag + raw bytes", [] {
        SerBuffer buf{from_hex("aabbcc")};
        Writer w;
        buf.write_to(w);
        KNC_CHECK_EQ_BYTES(std::move(w).take(), from_hex("00000003 00 aabbcc"),
                           "SerBuffer layout");
    });

    s.run("SerBuffer round-trips", [] {
        SerBuffer buf{from_ascii("hello world")};
        Writer w;
        buf.write_to(w);
        const Bytes wire = std::move(w).take();
        Reader r{view(wire)};
        const SerBuffer back = SerBuffer::read_from(r);
        KNC_CHECK_EQ_BYTES(back.data(), from_ascii("hello world"), "round-trip body");
        KNC_CHECK_EQ(back.compressed(), false, "round-trip flag");
        KNC_CHECK(r.exhausted(), "round-trip consumed every byte");
    });

    s.run("compress/decompress is lossless", [] {
        Bytes payload;
        for (int i = 0; i < 4096; ++i) {
            payload.push_back(static_cast<std::uint8_t>(i % 7));
        }
        SerBuffer buf{payload};
        buf.compress();
        KNC_CHECK(buf.compressed(), "compressed flag set");
        KNC_CHECK(buf.size() < payload.size(), "compression actually shrank it");
        buf.decompress();
        KNC_CHECK_EQ(buf.compressed(), false, "flag cleared");
        KNC_CHECK_EQ_BYTES(buf.data(), payload, "decompressed payload matches");
    });

    s.run("compressed body starts with a NATIVE-endian original size", [] {
        // KSerBuffer::Compress writes dwOriginSize with a raw 4-byte Write(),
        // so it is little-endian - inside a structure whose outer length is
        // big-endian. This mixed endianness is real; pin it.
        SerBuffer buf{Bytes(0x0102, 0x5A)};  // 258 bytes of 'Z'
        buf.compress();
        const Bytes& body = buf.data();
        KNC_CHECK_EQ_BYTES(Bytes(body.begin(), body.begin() + 4),
                           from_hex("02010000"), "LE original size header");
        // zlib stream follows: 0x78 is the standard zlib CMF byte.
        KNC_CHECK_EQ(static_cast<int>(body[4]), 0x78, "zlib header byte");
    });

    s.run("compress of an empty buffer still round-trips", [] {
        SerBuffer buf;
        buf.compress();
        KNC_CHECK(buf.compressed(), "empty buffer marked compressed");
        buf.decompress();
        KNC_CHECK(buf.empty(), "empty after decompress");
    });

    s.run("compress is idempotent and decompress is a no-op when plain", [] {
        SerBuffer buf{from_ascii("abc")};
        buf.decompress();  // no-op
        KNC_CHECK_EQ_BYTES(buf.data(), from_ascii("abc"), "decompress no-op");
        buf.compress();
        const Bytes once = buf.data();
        buf.compress();  // no-op
        KNC_CHECK_EQ_BYTES(buf.data(), once, "compress is idempotent");
    });

    s.run("absurd declared original size is rejected", [] {
        SerBuffer buf{from_ascii("x")};
        buf.compress();
        Bytes poisoned = buf.data();
        poisoned[0] = 0xFF;
        poisoned[1] = 0xFF;
        poisoned[2] = 0xFF;
        poisoned[3] = 0xFF;  // claims 4 GiB
        // Rebuild through the wire path so `compressed_` is set.
        Writer w;
        w.put_dword(static_cast<std::uint32_t>(poisoned.size()));
        w.put_bool(true);
        w.put_raw(view(poisoned));
        const Bytes wire = std::move(w).take();
        Reader r{view(wire)};
        SerBuffer back = SerBuffer::read_from(r);
        KNC_CHECK_THROWS(back.decompress(), "4 GiB original size must throw");
    });

    s.run("declared length beyond the buffer is rejected", [] {
        const Bytes wire = from_hex("000000ff 00 aabb");
        Reader r{view(wire)};
        KNC_CHECK_THROWS(SerBuffer::read_from(r), "truncated SerBuffer must throw");
    });

    s.run("corrupt zlib stream is rejected", [] {
        SerBuffer buf{Bytes(512, 0x41)};
        buf.compress();
        Bytes poisoned = buf.data();
        poisoned[8] ^= 0xFF;  // flip a byte inside the deflate stream
        Writer w;
        w.put_dword(static_cast<std::uint32_t>(poisoned.size()));
        w.put_bool(true);
        w.put_raw(view(poisoned));
        const Bytes wire = std::move(w).take();
        Reader r{view(wire)};
        SerBuffer back = SerBuffer::read_from(r);
        KNC_CHECK_THROWS(back.decompress(), "corrupt deflate stream must throw");
    });

    s.run("reader() refuses a still-compressed buffer", [] {
        SerBuffer buf{Bytes(64, 0x01)};
        buf.compress();
        KNC_CHECK_THROWS(buf.reader(), "reader() on compressed must throw");
    });
}

// ===========================================================================
// SecurityAssociation
// ===========================================================================

void test_security_association(knctest::Suite& s) {
    s.run("bootstrap SA keys are the exact CP949 bytes", [] {
        const SecurityAssociation sa;
        KNC_CHECK_EQ_BYTES(sa.auth_key(), from_hex("c0d3bdc3b7ceb8b8"),
                           "bootstrap auth key");
        KNC_CHECK_EQ_BYTES(sa.crypto_key(), from_hex("c7d8c4bfb5e9c0fd"),
                           "bootstrap crypto key");
        KNC_CHECK_EQ(sa.sequence_num(), SeqType{1}, "initial sequence num");
        KNC_CHECK_EQ(sa.last_sequence_num(), SeqType{0}, "initial last sequence");
    });

    s.run("sequence increment skips 0 on wrap", [] {
        SecurityAssociation sa;
        sa.set_sequence_num(std::numeric_limits<SeqType>::max());
        sa.increment_sequence_num();
        KNC_CHECK_EQ(sa.sequence_num(), SeqType{1}, "wraps to 1, not 0");
    });

    s.run("sequence validation follows IsValidSequenceNum_notRWM", [] {
        SecurityAssociation sa;
        KNC_CHECK_EQ(sa.accept_sequence_num(0), false, "seq 0 rejected");
        KNC_CHECK_EQ(sa.accept_sequence_num(1), true, "seq 1 accepted");
        KNC_CHECK_EQ(sa.accept_sequence_num(1), false, "replay rejected");
        KNC_CHECK_EQ(sa.accept_sequence_num(5), true, "forward jump accepted");
        KNC_CHECK_EQ(sa.accept_sequence_num(4), false, "stale rejected");
        KNC_CHECK_EQ(sa.last_sequence_num(), SeqType{5}, "last unchanged by reject");
    });

    s.run("only a wrap from SeqType max admits a lower number", [] {
        SecurityAssociation sa;
        sa.set_last_sequence_num(std::numeric_limits<SeqType>::max());
        KNC_CHECK_EQ(sa.accept_sequence_num(1), true, "wrap accepted");
        KNC_CHECK_EQ(sa.last_sequence_num(), SeqType{1}, "last follows the wrap");
    });

    s.run("SA serializes as authKey|cryptoKey|seq|lastSeq|mask", [] {
        SecurityAssociation sa;
        Writer w;
        sa.write_to(w);
        KNC_CHECK_EQ_BYTES(std::move(w).take(),
                           from_hex("00000008 c0d3bdc3b7ceb8b8"
                                    "00000008 c7d8c4bfb5e9c0fd"
                                    "00000001"
                                    "00000000"
                                    "00000000"),
                           "SA wire layout");
    });

    s.run("SA round-trips", [] {
        SecurityAssociation sa{from_hex("0001020304050607"),
                               from_hex("08090a0b0c0d0e0f")};
        sa.set_sequence_num(0x11223344);
        sa.set_last_sequence_num(0x55667788);
        Writer w;
        sa.write_to(w);
        const Bytes wire = std::move(w).take();
        Reader r{view(wire)};
        const SecurityAssociation back = SecurityAssociation::read_from(r);
        KNC_CHECK(back == sa, "SA round-trip");
        KNC_CHECK(r.exhausted(), "SA consumed every byte");
    });

    s.run("random SA produces distinct 8-byte keys", [] {
        const SecurityAssociation a = SecurityAssociation::random();
        const SecurityAssociation b = SecurityAssociation::random();
        KNC_CHECK_EQ(a.auth_key().size(), std::size_t{8}, "auth key size");
        KNC_CHECK_EQ(a.crypto_key().size(), std::size_t{8}, "crypto key size");
        KNC_CHECK(a.auth_key() != b.auth_key(), "two random SAs differ");
    });

    s.run("wrong key size is rejected", [] {
        KNC_CHECK_THROWS((SecurityAssociation{from_hex("0001"), from_hex("08090a0b0c0d0e0f")}),
                         "short auth key must throw");
    });
}

// ===========================================================================
// SecureBuffer seal/open
// ===========================================================================

void test_secure_buffer(knctest::Suite& s) {
    s.run("seal produces the documented layout", [] {
        SecurityAssociation sa;  // bootstrap
        const Bytes payload = from_ascii("hi");
        const Bytes iv = iv_of('A');
        const Bytes frame = seal_with_iv(sa, 0, view(payload), view(iv));

        // 2 SPI + 4 seq + 8 IV + 8 cipher (2 payload + 5 pad + 1 len) + 10 ICV
        KNC_CHECK_EQ(frame.size(), std::size_t{32}, "sealed size");
        KNC_CHECK_EQ_BYTES(Bytes(frame.begin(), frame.begin() + 2), from_hex("0000"),
                           "SPI 0 little-endian");
        KNC_CHECK_EQ_BYTES(Bytes(frame.begin() + 2, frame.begin() + 6),
                           from_hex("01000000"), "sequence 1 little-endian");
        KNC_CHECK_EQ_BYTES(Bytes(frame.begin() + 6, frame.begin() + 14), iv,
                           "IV echoed in clear");
    });

    s.run("seal/open round-trips at every length mod 8", [] {
        for (std::size_t n = 0; n <= 24; ++n) {
            SecurityAssociation tx;
            SecurityAssociation rx;
            Bytes payload(n);
            for (std::size_t i = 0; i < n; ++i) {
                payload[i] = static_cast<std::uint8_t>(0xC0 + i);
            }
            const Bytes iv = iv_of(static_cast<std::uint8_t>('A' + (n % 40)));
            const Bytes frame = seal_with_iv(tx, 1234, view(payload), view(iv));

            // Ciphertext length must always be a multiple of the block size.
            const std::size_t cipher_len = frame.size() - 6 - kIvSize - kIcvSize;
            KNC_CHECK_EQ(cipher_len % kBlockSize, std::size_t{0},
                         "ciphertext is block-aligned for n=" + std::to_string(n));

            const OpenResult out = open(rx, view(frame), false);
            KNC_CHECK(static_cast<bool>(out),
                      "open succeeded for n=" + std::to_string(n));
            KNC_CHECK_EQ_BYTES(out.payload, payload,
                               "payload survives for n=" + std::to_string(n));
        }
    });

    s.run("padding is exactly 1..padLen followed by padLen", [] {
        // A 7-byte payload needs padLen = 8 - ((7+1) % 8) = 8, giving a
        // 16-byte plaintext: 7 payload + 8 pad + 1 length byte.
        SecurityAssociation sa;
        const Bytes payload = from_hex("00112233445566");
        const Bytes iv = iv_of('A');
        const Bytes frame = seal_with_iv(sa, 1, view(payload), view(iv));
        const std::size_t cipher_len = frame.size() - 6 - kIvSize - kIcvSize;
        KNC_CHECK_EQ(cipher_len, std::size_t{16}, "7-byte payload -> 16-byte cipher");

        // Decrypt by hand and inspect the trailer.
        const ByteView cipher{frame.data() + 6 + kIvSize, cipher_len};
        const Bytes plain = sa.decrypt(cipher, view(iv));
        KNC_CHECK_EQ_BYTES(Bytes(plain.begin() + 7, plain.end()),
                           from_hex("01020304050607 08 08"),
                           "pad bytes 1..8 then padLen 8");
    });

    s.run("a payload that is already 7 mod 8 gets a single pad byte", [] {
        // padLen = 8 - ((n+1) % 8); n = 6 -> 8 - 7 = 1.
        SecurityAssociation sa;
        const Bytes payload = from_hex("001122334455");
        const Bytes iv = iv_of('B');
        const Bytes frame = seal_with_iv(sa, 1, view(payload), view(iv));
        const std::size_t cipher_len = frame.size() - 6 - kIvSize - kIcvSize;
        KNC_CHECK_EQ(cipher_len, std::size_t{8}, "6-byte payload -> 8-byte cipher");
        const ByteView cipher{frame.data() + 6 + kIvSize, cipher_len};
        const Bytes plain = sa.decrypt(cipher, view(iv));
        KNC_CHECK_EQ_BYTES(Bytes(plain.begin() + 6, plain.end()), from_hex("0101"),
                           "single pad byte then padLen 1");
    });

    s.run("a flipped bit anywhere fails the ICV", [] {
        SecurityAssociation tx;
        const Bytes payload = from_ascii("the quick brown fox");
        const Bytes frame = seal_with_iv(tx, 7, view(payload), view(iv_of('C')));
        for (std::size_t i = 0; i < frame.size(); ++i) {
            SecurityAssociation rx;
            Bytes bad = frame;
            bad[i] ^= 0x01;
            const OpenResult out = open(rx, view(bad), false);
            if (out) {
                KNC_CHECK(false, "byte " + std::to_string(i) +
                                     " flipped but the frame still opened");
            }
        }
        KNC_CHECK(true, "every single-bit flip was rejected");
    });

    s.run("a truncated frame is rejected", [] {
        SecurityAssociation tx;
        const Bytes frame = seal_with_iv(tx, 7, view(from_ascii("x")), view(iv_of('D')));
        for (std::size_t n = 0; n < kMinSecureSize; ++n) {
            SecurityAssociation rx;
            const OpenResult out = open(rx, ByteView{frame.data(), n}, false);
            if (out.error != OpenError::kTooShort) {
                KNC_CHECK(false, "length " + std::to_string(n) +
                                     " should be kTooShort");
            }
        }
        KNC_CHECK(true, "all sub-minimum lengths rejected as kTooShort");
    });

    s.run("a wrong key fails the ICV, never the padding", [] {
        SecurityAssociation tx;
        const Bytes frame = seal_with_iv(tx, 7, view(from_ascii("secret")),
                                         view(iv_of('E')));
        SecurityAssociation rx{from_hex("0000000000000000"),
                               from_hex("0000000000000000")};
        const OpenResult out = open(rx, view(frame), false);
        KNC_CHECK_EQ(static_cast<int>(out.error), static_cast<int>(OpenError::kBadIcv),
                     "wrong key -> kBadIcv (authenticate before decrypting)");
    });

    s.run("sequence numbers advance only when SPI != 0", [] {
        SecurityAssociation boot;
        (void)seal_with_iv(boot, 0, view(from_ascii("a")), view(iv_of('A')));
        (void)seal_with_iv(boot, 0, view(from_ascii("b")), view(iv_of('A')));
        KNC_CHECK_EQ(boot.sequence_num(), SeqType{1}, "SPI 0 keeps sequence at 1");

        SecurityAssociation sess;
        (void)seal_with_iv(sess, 42, view(from_ascii("a")), view(iv_of('A')));
        KNC_CHECK_EQ(sess.sequence_num(), SeqType{2}, "SPI != 0 advances to 2");
        (void)seal_with_iv(sess, 42, view(from_ascii("b")), view(iv_of('A')));
        KNC_CHECK_EQ(sess.sequence_num(), SeqType{3}, "and to 3");
    });

    s.run("sequence checking rejects a replayed frame", [] {
        SecurityAssociation tx;
        SecurityAssociation rx;
        const Bytes first = seal_with_iv(tx, 9, view(from_ascii("one")), view(iv_of('A')));
        const Bytes second = seal_with_iv(tx, 9, view(from_ascii("two")), view(iv_of('A')));

        KNC_CHECK(static_cast<bool>(open(rx, view(first), true)), "first accepted");
        KNC_CHECK(static_cast<bool>(open(rx, view(second), true)), "second accepted");
        const OpenResult replay = open(rx, view(first), true);
        KNC_CHECK_EQ(static_cast<int>(replay.error),
                     static_cast<int>(OpenError::kReplayedSequence),
                     "replay of the first frame rejected");
    });

    s.run("sequence checking off accepts out-of-order frames", [] {
        SecurityAssociation tx;
        SecurityAssociation rx;
        const Bytes first = seal_with_iv(tx, 9, view(from_ascii("one")), view(iv_of('A')));
        const Bytes second = seal_with_iv(tx, 9, view(from_ascii("two")), view(iv_of('A')));
        KNC_CHECK(static_cast<bool>(open(rx, view(second), false)), "second first");
        KNC_CHECK(static_cast<bool>(open(rx, view(first), false)), "then first");
    });

    s.run("the SPI on the wire is read but never enforced", [] {
        // Legacy behaviour: each side decrypts with its own stored SA, so a
        // mismatched SPI field must NOT cause a rejection.
        SecurityAssociation tx;
        const Bytes frame = seal_with_iv(tx, 0xBEEF, view(from_ascii("ok")),
                                         view(iv_of('F')));
        KNC_CHECK_EQ(peek_spi(view(frame)), SpiType{0xBEEF}, "SPI readable");
        SecurityAssociation rx;  // same keys, different notion of "its" SPI
        KNC_CHECK(static_cast<bool>(open(rx, view(frame), false)),
                  "mismatched SPI still opens");
    });

    s.run("the random IV path still round-trips", [] {
        SecurityAssociation tx;
        SecurityAssociation rx;
        const Bytes payload = from_ascii("random iv please");
        const Bytes a = seal(tx, 3, view(payload));
        const Bytes b = seal(tx, 3, view(payload));
        KNC_CHECK(a != b, "two seals of the same payload differ");
        const OpenResult out = open(rx, view(a), false);
        KNC_CHECK(static_cast<bool>(out), "random-IV frame opens");
        KNC_CHECK_EQ_BYTES(out.payload, payload, "random-IV payload matches");
    });

    s.run("DES-CBC chains: identical blocks encrypt differently", [] {
        SecurityAssociation sa;
        const Bytes payload(32, 0x00);
        const Bytes frame = seal_with_iv(sa, 1, view(payload), view(iv_of('A')));
        const std::size_t cipher_off = 6 + kIvSize;
        const Bytes b0(frame.begin() + cipher_off, frame.begin() + cipher_off + 8);
        const Bytes b1(frame.begin() + cipher_off + 8, frame.begin() + cipher_off + 16);
        KNC_CHECK(b0 != b1, "CBC chaining is active (not ECB)");
    });
}

// ===========================================================================
// SecureChannel + handshake
// ===========================================================================

void test_secure_channel(knctest::Suite& s) {
    s.run("a new channel starts on SPI 0 with the bootstrap keys", [] {
        SecureChannel ch;
        KNC_CHECK_EQ(ch.spi(), SpiType{0}, "starts on SPI 0");
        KNC_CHECK(ch.on_bootstrap_sa(), "starts on the bootstrap SA");
        KNC_CHECK_EQ_BYTES(ch.sa().auth_key(), from_hex("c0d3bdc3b7ceb8b8"),
                           "bootstrap auth key");
    });

    s.run("sequence checking stays off while on the bootstrap SA", [] {
        SecureChannel tx;
        SecureChannel rx;
        rx.set_check_sequence(true);
        const Bytes a = tx.seal(view(from_ascii("one")));
        const Bytes b = tx.seal(view(from_ascii("two")));
        // Both carry sequence 1 because SPI 0 never advances; with the legacy
        // rule they must both still be accepted.
        KNC_CHECK(static_cast<bool>(rx.open(view(a))), "first bootstrap frame");
        KNC_CHECK(static_cast<bool>(rx.open(view(b))), "second bootstrap frame");
    });

    s.run("full handshake: SPI 0 carries the session SA", [] {
        // --- server side -------------------------------------------------
        SecureChannel server;
        const FreshSa fresh = create_sa();
        KNC_CHECK(fresh.spi != 0, "minted SPI is never 0");

        Writer body;
        body.put_ushort(fresh.spi);  // NOTE: big-endian here, LE in the header
        fresh.sa.write_to(body);
        const Bytes handshake_payload = std::move(body).take();

        // Sealed under the bootstrap SA, THEN the server adopts the new SA -
        // exactly the ordering in KSession::OnAcceptConnection.
        const Bytes sealed = server.seal(view(handshake_payload));
        server.adopt(fresh.spi, fresh.sa);

        KNC_CHECK_EQ(peek_spi(view(sealed)), SpiType{0},
                     "handshake frame is sent under SPI 0");

        // --- client side ---------------------------------------------------
        SecureChannel client;  // also starts on the bootstrap SA
        const OpenResult opened = client.open(view(sealed));
        KNC_CHECK(static_cast<bool>(opened), "client opens the handshake");

        Reader r{view(opened.payload)};
        const SpiType spi = r.get_ushort();
        const SecurityAssociation sa = SecurityAssociation::read_from(r);
        KNC_CHECK(r.exhausted(), "handshake body fully consumed");
        KNC_CHECK_EQ(spi, fresh.spi, "client learns the SPI");
        KNC_CHECK(sa == fresh.sa, "client learns the SA");
        client.adopt(spi, sa);

        // --- both sides now talk under the session SA ----------------------
        const Bytes msg = client.seal(view(from_ascii("EGS_CONNECT_REQ")));
        KNC_CHECK_EQ(peek_spi(view(msg)), fresh.spi, "client sends the new SPI");
        const OpenResult back = server.open(view(msg));
        KNC_CHECK(static_cast<bool>(back), "server opens the session frame");
        KNC_CHECK_EQ_BYTES(back.payload, from_ascii("EGS_CONNECT_REQ"),
                           "session payload survives");
    });

    s.run("the SPI appears big-endian in the body, little-endian in the header", [] {
        SecureChannel server;
        Writer body;
        body.put_ushort(0x1234);
        const Bytes payload = std::move(body).take();
        KNC_CHECK_EQ_BYTES(payload, from_hex("1234"), "body SPI is big-endian");
        const Bytes sealed = server.seal_with_iv(view(payload), view(iv_of('A')));
        KNC_CHECK_EQ_BYTES(Bytes(sealed.begin(), sealed.begin() + 2),
                           from_hex("0000"), "header SPI is little-endian");
    });
}

// ===========================================================================
// Framing
// ===========================================================================

void test_framing(knctest::Suite& s) {
    s.run("the length prefix is total size, little-endian", [] {
        const Bytes body(30, 0xAB);
        const Bytes framed = frame(view(body));
        KNC_CHECK_EQ(framed.size(), std::size_t{32}, "framed size");
        KNC_CHECK_EQ_BYTES(Bytes(framed.begin(), framed.begin() + 2),
                           from_hex("2000"), "length 32 little-endian");
    });

    s.run("a frame round-trips through the reader", [] {
        const Bytes body(40, 0xCD);
        const Bytes framed = frame(view(body));
        FrameReader reader;
        reader.push(view(framed));
        const auto next = reader.next();
        KNC_CHECK(next.has_frame, "frame recognised");
        KNC_CHECK_EQ_BYTES(Bytes(next.body.begin(), next.body.end()), body,
                           "body matches");
        KNC_CHECK_EQ(reader.next().has_frame, false, "no second frame");
    });

    s.run("frames arriving byte-by-byte still assemble", [] {
        const Bytes body(27, 0x5A);
        const Bytes framed = frame(view(body));
        FrameReader reader;
        for (std::size_t i = 0; i + 1 < framed.size(); ++i) {
            reader.push(ByteView{framed.data() + i, 1});
            if (reader.next().has_frame) {
                KNC_CHECK(false, "frame completed too early at byte " +
                                     std::to_string(i));
            }
        }
        reader.push(ByteView{framed.data() + framed.size() - 1, 1});
        const auto next = reader.next();
        KNC_CHECK(next.has_frame, "frame completes on the last byte");
        KNC_CHECK_EQ(next.body.size(), body.size(), "dribbled body size");
    });

    s.run("several frames in one read are all delivered", [] {
        Bytes stream;
        for (int i = 0; i < 5; ++i) {
            const Bytes body(30 + static_cast<std::size_t>(i), static_cast<std::uint8_t>(i));
            append(stream, view(frame(view(body))));
        }
        FrameReader reader;
        reader.push(view(stream));
        int count = 0;
        while (true) {
            const auto next = reader.next();
            if (!next.has_frame) {
                KNC_CHECK_EQ(next.failed(), false, "no error while draining");
                break;
            }
            KNC_CHECK_EQ(next.body.size(), static_cast<std::size_t>(30 + count),
                         "frame " + std::to_string(count) + " size");
            ++count;
        }
        KNC_CHECK_EQ(count, 5, "all five frames delivered");
        KNC_CHECK_EQ(reader.buffered(), std::size_t{0}, "buffer drained");
    });

    s.run("a zero length is rejected instead of spinning forever", [] {
        // The legacy loop would do `m_dwLeft -= 0` and never progress.
        FrameReader reader;
        reader.push(view(from_hex("0000 0000")));
        const auto next = reader.next();
        KNC_CHECK_EQ(next.has_frame, false, "no frame produced");
        KNC_CHECK_EQ(static_cast<int>(next.error),
                     static_cast<int>(FrameError::kLengthTooSmall),
                     "zero length flagged");
    });

    s.run("a length below the secure-buffer minimum is rejected", [] {
        FrameReader reader;
        reader.push(view(from_hex("0500 0000 0000")));  // declares 5 bytes
        KNC_CHECK_EQ(static_cast<int>(reader.next().error),
                     static_cast<int>(FrameError::kLengthTooSmall),
                     "sub-minimum length flagged");
    });

    s.run("an oversize length is rejected immediately, not after buffering", [] {
        FrameReader reader;
        reader.push(view(from_hex("0180")));  // 0x8001 = 32769 > MAX_PACKET_SIZE
        const auto next = reader.next();
        KNC_CHECK_EQ(static_cast<int>(next.error),
                     static_cast<int>(FrameError::kLengthTooLarge),
                     "oversize length flagged on the first two bytes");
    });

    s.run("the maximum legal frame is accepted", [] {
        const Bytes body(kMaxPacketSize - 2, 0x11);
        const Bytes framed = frame(view(body));
        KNC_CHECK_EQ(framed.size(), kMaxPacketSize, "exactly MAX_PACKET_SIZE");
        FrameReader reader;
        reader.push(view(framed));
        KNC_CHECK(reader.next().has_frame, "max-size frame accepted");
    });

    s.run("an outbound frame over the limit throws rather than being sent", [] {
        const Bytes body(kMaxPacketSize - 1, 0x11);
        KNC_CHECK_THROWS(frame(view(body)), "oversize outbound frame must throw");
    });

    s.run("end-to-end: event -> seal -> frame -> parse", [] {
        SecureChannel tx;
        SecureChannel rx;

        Event out{Ev::EGS_CONNECT_REQ};
        out.dest().performer_id = 0x0102;
        out.dest().add_uid(42);
        out.push_trace(7);
        Writer body;
        body.put_wstring(u"1.0.0");
        out.payload().assign(std::move(body).take());

        const Bytes wire = frame(view(tx.seal(view(out.serialize()))));

        FrameReader reader;
        reader.push(view(wire));
        const auto next = reader.next();
        KNC_CHECK(next.has_frame, "frame assembled");
        const OpenResult opened = rx.open(next.body);
        KNC_CHECK(static_cast<bool>(opened), "frame opened");
        const Event back = Event::deserialize(view(opened.payload));
        KNC_CHECK_EQ(back.id(), raw(Ev::EGS_CONNECT_REQ), "event id survives");
        KNC_CHECK_EQ(back.dest().performer_id, PerformerId{0x0102}, "performer id");
        KNC_CHECK(back.dest().has_uid(42), "uid survives");
        KNC_CHECK_EQ(back.first_sender(), UidType{7}, "trace survives");
        Reader pr = back.payload().reader();
        KNC_CHECK(pr.get_wstring() == std::u16string(u"1.0.0"), "body survives");
    });
}

// ===========================================================================
// Event
// ===========================================================================

void test_event(knctest::Suite& s) {
    s.run("an empty event has the documented 26-byte layout", [] {
        Event e{Ev::E_HEART_BEAT};
        KNC_CHECK_EQ_BYTES(e.serialize(),
                           from_hex("00000000"            // performer id 0
                                    "00000000"            // uid count 0
                                    "ffffffffffffffff"    // trace[0] = -1
                                    "ffffffffffffffff"    // trace[1] = -1
                                    "0000"                // event id 0
                                    "00000000"),          // empty SerBuffer
                           "empty event layout");
    });

    s.run("uids serialize ascending after a BE count", [] {
        Event e{Ev::EGS_CONNECT_REQ};
        e.dest().performer_id = 0x0202;
        e.dest().add_uid(3);
        e.dest().add_uid(1);
        e.dest().add_uid(2);
        const Bytes wire = e.serialize();
        KNC_CHECK_EQ_BYTES(Bytes(wire.begin(), wire.begin() + 32),
                           from_hex("00000202"
                                    "00000003"
                                    "0000000000000001"
                                    "0000000000000002"
                                    "0000000000000003"),
                           "performer info layout");
        KNC_CHECK_EQ_BYTES(Bytes(wire.begin() + 48, wire.begin() + 50),
                           from_hex("001a"), "EGS_CONNECT_REQ == 26, big-endian");
    });

    s.run("trace push/pop follows GetLastSenderUID", [] {
        Event e;
        KNC_CHECK(e.trace_empty(), "fresh event has an empty trace");
        KNC_CHECK_EQ(e.last_sender(), kNoUid, "no sender yet");
        e.push_trace(100);
        KNC_CHECK_EQ(e.first_sender(), UidType{100}, "first sender");
        KNC_CHECK_EQ(e.last_sender(), UidType{100}, "last falls back to first");
        e.push_trace(200);
        KNC_CHECK_EQ(e.first_sender(), UidType{100}, "first unchanged");
        KNC_CHECK_EQ(e.last_sender(), UidType{200}, "last is the second hop");
        e.pop_trace();
        KNC_CHECK_EQ(e.last_sender(), UidType{100}, "pop returns to one hop");
        e.pop_trace();
        KNC_CHECK(e.trace_empty(), "pop twice empties the trace");
    });

    s.run("event round-trips with a payload", [] {
        Event e{Ev::EGS_VERIFY_ACCOUNT_REQ};
        e.dest().performer_id = 0x0502;
        e.dest().add_uid(-1);
        e.set_trace(11, 22);
        Writer body;
        body.put_wstring(u"player");
        body.put_int(7);
        e.payload().assign(std::move(body).take());

        const Event back = Event::deserialize(view(e.serialize()));
        KNC_CHECK_EQ(back.id(), raw(Ev::EGS_VERIFY_ACCOUNT_REQ), "event id");
        KNC_CHECK_EQ(back.dest().performer_id, PerformerId{0x0502}, "performer id");
        KNC_CHECK(back.dest().has_uid(-1), "uid -1 survives");
        KNC_CHECK_EQ(back.trace(0), UidType{11}, "trace[0]");
        KNC_CHECK_EQ(back.trace(1), UidType{22}, "trace[1]");
        Reader r = back.payload().reader();
        KNC_CHECK(r.get_wstring() == std::u16string(u"player"), "payload wstring");
        KNC_CHECK_EQ(r.get_int(), 7, "payload int");
        KNC_CHECK(r.exhausted(), "payload fully consumed");
    });

    s.run("a compressed payload round-trips through the event", [] {
        Event e{Ev::EGS_CONNECT_ACK};
        Writer body;
        for (int i = 0; i < 2000; ++i) {
            body.put_int(i % 3);
        }
        e.payload().assign(std::move(body).take());
        const std::size_t plain_size = e.payload().size();
        e.payload().compress();
        KNC_CHECK(e.payload().size() < plain_size, "payload compressed");

        Event back = Event::deserialize(view(e.serialize()));
        KNC_CHECK(back.payload().compressed(), "arrives compressed");
        back.payload().decompress();
        KNC_CHECK_EQ(back.payload().size(), plain_size, "restores the exact size");
        Reader r = back.payload().reader();
        for (int i = 0; i < 2000; ++i) {
            if (r.get_int() != i % 3) {
                KNC_CHECK(false, "compressed payload corrupted at " + std::to_string(i));
                return;
            }
        }
        KNC_CHECK(true, "2000 compressed ints survive intact");
    });

    s.run("MAX_UID_NUM is enforced", [] {
        PerformerInfo info;
        for (int i = 0; i < 2000; ++i) {
            KNC_CHECK_EQ(info.add_uid(i), true, "");
        }
        KNC_CHECK_EQ(info.uids.size(), std::size_t{2000}, "2000 uids accepted");
        KNC_CHECK_EQ(info.add_uid(9999), false, "the 2001st uid is refused");
    });

    s.run("event ids resolve to their legacy names", [] {
        KNC_CHECK(std::string{event_name(0)} == "E_HEART_BEAT", "id 0");
        KNC_CHECK(std::string{event_name(1)} == "E_ACCEPT_CONNECTION_NOT", "id 1");
        KNC_CHECK(std::string{event_name(26)} == "EGS_CONNECT_REQ", "id 26");
        KNC_CHECK(std::string{event_name(27)} == "EGS_CONNECT_ACK", "id 27");
        KNC_CHECK(std::string{event_name(60000)} == "EVENT_EVENTID_SENTINEL",
                  "out-of-range id clamps to the sentinel");
    });

    s.run("client-originable ids stop at EGS_CLIENT_EVENT_ID_END", [] {
        KNC_CHECK(is_client_originable(raw(Ev::EGS_CONNECT_REQ)), "client event ok");
        KNC_CHECK(is_client_originable(kClientEventEnd), "boundary is inclusive");
        KNC_CHECK(!is_client_originable(kClientEventEnd + 1),
                  "the first server event is refused");
    });

    s.run("a truncated event is rejected", [] {
        Event e{Ev::EGS_CONNECT_REQ};
        const Bytes wire = e.serialize();
        for (std::size_t n = 0; n < wire.size(); ++n) {
            bool threw = false;
            try {
                (void)Event::deserialize(ByteView{wire.data(), n});
            } catch (const ProtocolError&) {
                threw = true;
            }
            if (!threw) {
                KNC_CHECK(false,
                          "truncation to " + std::to_string(n) + " bytes was accepted");
                return;
            }
        }
        KNC_CHECK(true, "every truncation was rejected");
    });

    s.run("a hostile uid count cannot allocate", [] {
        const Bytes wire = from_hex("00000000 ffffffff");
        KNC_CHECK_THROWS(Event::deserialize(view(wire)),
                         "4-billion uid count must throw");
    });
}

}  // namespace

int main() {
    knctest::Suite suite{"Packet envelope"};
    test_serbuffer(suite);
    test_security_association(suite);
    test_secure_buffer(suite);
    test_secure_channel(suite);
    test_framing(suite);
    test_event(suite);
    return suite.summary();
}
