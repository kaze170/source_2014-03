#include "kproto/Event.h"

#include <sstream>

namespace knc {

bool PerformerInfo::addUid(UidType uid) {
    if (uids.size() >= kMaxUidNum) return false;
    uids.insert(uid);
    return true;
}

bool PerformerInfo::findUid(UidType uid) const { return uids.find(uid) != uids.end(); }

UidType PerformerInfo::firstUid() const { return uids.empty() ? kInvalidUid : *uids.begin(); }

void PerformerInfo::serialize(Serializer& s) {
    s.io(performerId);
    s.io(uids);
}

void Event::pushTrace(UidType uid) {
    trace[1] = trace[0];
    trace[0] = uid;
}

void Event::popTrace() {
    trace[0] = trace[1];
    trace[1] = kInvalidUid;
}

void Event::serialize(Serializer& s) {
    destPerformer.serialize(s);
    s.io(trace[0]);
    s.io(trace[1]);
    s.io(eventId);
    s.io(payload);
}

std::string Event::describe() const {
    std::ostringstream os;
    os << eventIdName(eventId) << '(' << eventId << ") perf=" << destPerformer.performerId
       << " uids=" << destPerformer.uidCount() << " trace=[" << trace[0] << ',' << trace[1]
       << "] bytes=" << payload.size();
    return os.str();
}

}  // namespace knc
