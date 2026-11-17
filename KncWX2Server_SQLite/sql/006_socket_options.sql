-- Wire option IDs are independent of ItemSockets' computed stat kind and value.
-- Both original arrays have five slots (GSGameDBThread.cpp:9189, :9258).
CREATE TABLE IF NOT EXISTS ItemSocketOptions (
    ItemUID INTEGER NOT NULL REFERENCES Items(ItemUID) ON DELETE CASCADE,
    SocketKind INTEGER NOT NULL CHECK (SocketKind IN (0, 1)),
    SocketIndex INTEGER NOT NULL CHECK (SocketIndex BETWEEN 0 AND 4),
    OptionID INTEGER NOT NULL DEFAULT 0,
    PRIMARY KEY (ItemUID, SocketKind, SocketIndex)
);
