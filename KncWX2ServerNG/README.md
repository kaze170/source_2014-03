# KncWX2ServerNG

`KncWX2Server`（2014）的完全重寫：**單一行程**、**C++26 Modules**、**SQLite 3.54**，
並且必須與**未修改的原版客戶端**完全相容。

原版是五支獨立執行檔（LoginServer / GameServer / CenterServer / ChannelServer /
GlobalServer）、約 887k 行、靠 ODBC 連 MSSQL、用 Crypto++ 做封包加解密。
這裡把它收斂成一支行程、一個 SQLite 檔、零外部密碼學相依。

> 目標區域為 **TW/HK**（`SERV_COUNTRY_TW` → `SERV_COUNTRY_TWHK`）。

---

## 現在的狀態

協定核心已完成並通過驗證。這是整個專案風險最高的部分——它是跟客戶端之間
唯一不能改的契約，一個位元組錯掉，連線會成功、然後每個字串都是亂碼。

| 模組 | 內容 | 狀態 |
|---|---|---|
| `knc.core.types` | 舊版純量別名、`Bytes`/`ByteView`、`ProtocolError` | 完成 |
| `knc.crypto.des` | FIPS 46-3 DES + CBC（取代 Crypto++） | 6 組 NIST 向量通過 |
| `knc.crypto.md5` | MD5 + HMAC-MD5（可截斷） | RFC 1321 / 2202 向量通過 |
| `knc.proto.serializer` | `KSerializer` 線路格式（大端整數 / 小端 `wchar_t`） | 51 項檢查通過 |
| `knc.proto.serbuffer` | `KSerBuffer` + zlib 壓縮框架 | 通過 |
| `knc.proto.security` | SPI/Seq/IV/DES-CBC/ICV 信封、SA、金鑰交換 | 通過 |
| `knc.proto.framing` | TCP 長度前綴切包 | 通過 |
| `knc.proto.event_ids` | 3386 個事件 ID | 兩種獨立方法交叉驗證 |
| `knc.proto.event` | `KEvent` / `KPerformerInfo` | 通過 |
| `knc.proto.packet_io` | `Codec<T>`：C++ 型別 ↔ 線路格式的唯一對應表 | 通過 |
| `knc.proto.packets` | 1953 個封包結構體、7537 個欄位 | 兩種獨立方法交叉驗證 |
| `knc.proto.dispatch` | 3386 個事件 ID ↔ 封包結構體的綁定表 | 兩種獨立方法交叉驗證 |
| `knc.proto.session` | 整條收送路徑：分幀 → 信封 → 事件 → 封包 | 兩種獨立方法交叉驗證 |

**7 個測試套件、2575 項檢查、0 失敗。**

另外還有七組**差分交叉驗證**（`tools/crosscheck/`），用別人的實作來檢查我的：

| 腳本 | 對照組 | 結果 |
|---|---|---|
| `check_md5.sh` | Python `hashlib` / `hmac` | 1200 筆比對，0 不符 |
| `check_des.sh` | `pyDes`（獨立純 Python DES） | 600 筆比對，0 不符 |
| `check_envelope.sh` | 依原始碼另寫的 Python 信封參考實作 | 200 個封包，逐位元組相同 |
| `check_event_ids.sh` | 真正的 C++ 前處理器展開原版標頭 | 3386 列，0 不符 |
| `check_packets.sh` | 真正的 C++ **編譯器**跑原版序列化巨集 | 1953 型別 / 7537 欄位，0 不符 |
| `check_event_packets.sh` | `static_assert` 原版標頭 + 十個區域編譯後的**符號表** | 3386 筆綁定，0 不符 |
| `check_handshake.sh` | 另外用 Python 獨立實作**整個協定堆疊** | 完整登入對話，雙向逐位元組相同 |

尚未開始：遊戲邏輯、SQLite 資料層、IOCP 網路層。
詳見本頁最後的「後續工作」。

---

## 建置與測試

### 可攜驗證建置（任何 C++20 編譯器）

```sh
cd KncWX2ServerNG
tests/run_tests.sh
```

或透過 CMake：

```sh
cmake -S . -B build -DKNCNG_CLASSIC_MODE=ON
cmake --build build -j
ctest --test-dir build --output-on-failure
```

### 出貨建置（VC++26 / Visual Studio 2026）

```powershell
cmake -S . -B build -G "Visual Studio 18 2026" -A x64
cmake --build build --config Release
```

### 雙模式建置契約

每個 `.cppm` **同時**是：

* 真正的 C++26 module interface unit（MSVC 出貨用）；
* 定義 `KNC_CLASSIC` 時，一個 `#pragma once` 標頭（可攜驗證用）。

所以**測的就是出的那份程式碼**，沒有替身、沒有重新實作。
細節見 `src/knc_prelude.hpp` 與 `docs/VERIFICATION.md`。

---

## 文件

| 檔案 | 內容 |
|---|---|
| `docs/PROTOCOL.md` | **線路協定規格**——跟客戶端之間的硬契約，每一節都標註由哪個測試固定住 |
| `docs/VERIFICATION.md` | 驗證方法、交叉驗證程序、以及「還沒驗證什麼」的誠實清單 |

---

## 目錄結構

```
KncWX2ServerNG/
├── CMakeLists.txt
├── src/
│   ├── knc_prelude.hpp        雙模式前言，定義 KNC_EXPORT
│   ├── classic_tu.cpp         KNC_CLASSIC 模式下唯一的 TU
│   ├── core/types.cppm
│   ├── crypto/{des,md5}.cppm
│   └── proto/
│       ├── serializer.cppm    KSerializer 線路格式
│       ├── serbuffer.cppm     KSerBuffer + zlib
│       ├── security.cppm      ESP 風格信封 + SA + 金鑰交換
│       ├── framing.cppm       TCP 長度前綴
│       ├── event_ids.cppm     3386 個事件 ID
│       ├── event_ids.inc      產生檔，勿手改
│       ├── event.cppm         KEvent / KPerformerInfo
│       ├── packet_io.cppm     Codec<T>：型別 ↔ 線路格式
│       ├── packets.cppm       1953 個封包結構體
│       ├── packets/*.inc      產生檔，勿手改
│       ├── dispatch.cppm      事件 ID ↔ 封包綁定
│       ├── event_packets.inc  產生檔，勿手改
│       └── session.cppm       整條收送路徑與交握
├── tests/
│   ├── run_tests.sh
│   ├── test_util.hpp          無相依的極簡斷言框架
│   ├── test_crypto.cpp        DES / MD5 / HMAC 已知答案測試
│   ├── test_serializer.cpp    線路格式位元組級固定
│   ├── test_proto.cpp         信封 / 切包 / 事件
│   ├── test_event_ids.cpp     事件 ID 表結構不變量
│   ├── test_packets.cpp       封包位元組級固定 + 往返
│   ├── test_dispatch.cpp      事件 ID ↔ 封包綁定
│   └── test_session.cpp       交握與整條收送路徑
├── tools/
│   ├── gen_event_ids.py       從原版標頭產生 event_ids.inc
│   ├── gen_packets.py         從原版標頭產生 packets/*.inc
│   ├── gen_event_packets.py   產生事件 ID ↔ 封包綁定表
│   └── crosscheck/            七組差分交叉驗證腳本
└── third_party/
    ├── sqlite/                SQLite 3.54.1 amalgamation
    └── zlib/                  zlib 1.3.1
```

---

## 幾個關鍵決定

**為什麼自己實作 DES 和 MD5。**
原版用 Crypto++。那是一個大型相依，而我們只用到其中兩個演算法，
而且是不能更動的兩個。自己寫一份約 450 行、對著 NIST / RFC 向量釘死，
比起把整個 Crypto++ 拉進來更好管。**演算法本身一個位元都沒改**——
DES 和 MD5 以今天的標準都已經不安全，但它們在客戶端裡，所以不能動。

**為什麼事件 ID 是產生的、而且驗證兩次。**
3386 個 ID 必須和 2014 年編譯客戶端時完全一致。手抄一定會錯，
而錯誤會沉默地往後推移所有 ID。所以：Python regex 解析一次、
真正的 C++ 前處理器展開一次，兩份逐列比對，全部一致。

**為什麼砍掉全域 `KSADatabase`。**
原版是一個行程級的 `map` + `CRITICAL_SECTION`，每個封包都要進去查一次，
但每條連線其實只會用到自己那一筆 SA——而且斷線時從不刪除。
純粹的鎖競爭加上無上限洩漏。現在 SA 直接住在 session 裡。

**對原版行為的刻意偏離，只有十項，而且客戶端都觀察不到。**
IV 改用 CSPRNG（原版是 `'A' + rand() % 40` 重複八次）、
ICV 改成定時比較、長度為 0 的 TCP 封包不再讓接收迴圈無限空轉，等等。
完整清單與理由見 `docs/PROTOCOL.md` §8。

---

## 後續工作

依建議順序：

1. **錄一份原版客戶端的握手封包來重播。**
   目前的信封對著原始碼和公開密碼學向量是逐位元組正確的，
   封包結構也由兩條獨立路徑驗證過，
   但唯一能真正閉環的證據是拿真實客戶端的封包跑一次。
   這是目前最大的缺口，也是最值得做的下一個驗證。

2. **SQLite 資料層。**
   原版九種 DB worker thread（Account / Game / Log / Event / SMS / Billing /
   NXWeb / Relay / ChatLog）全部收斂到一個 SQLite 連線池。
   schema 來自 `Database/` 專案。

3. **IOCP 網路層 + session 管理。**
   `FrameReader` 與 `SecureChannel` 已經是 session 層需要的全部協定狀態。

4. **路由模型收斂。**
   `ePerformerID = SC_<server> | PC_<performer>`。單一行程下，
   `KncSend()` 的「跨伺服器」分支只剩下面向客戶端的 proxy 那一條。

6. **待確認：** TW/HK 下 `UDP_CAN_NOT_SEND_USER_KICK` 是否啟用——
   這決定 `KEGS_CONNECT_ACK` 結尾是否多一個 `bool`。

---

## 授權與第三方

* `third_party/sqlite` — SQLite 3.54.1 amalgamation，public domain。
  `SQLITE_SOURCE_ID = 2026-10-16 14:45:20 555e07ef33f69a0957d24d8e555b702464a131d6f1607f4c3c1e4551e8e6ca76`
* `third_party/zlib` — zlib 1.3.1，zlib license（見 `third_party/zlib/LICENSE`）。

兩者都刻意 vendor 進來：這個 repo 必須能離線、可重現地建置，
而 SQLite 的版本本身就是規格的一部分。
