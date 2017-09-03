namespace Server.Networking
{
    public enum OutgoingPacket
    {
        SendMessage,
        SendEnterGame,
        SendLeaveGame,
        SendIncomingFilesMeta,
        SendFileMeta,


        SendFileChunk,
        SendEndSendingFile,
        SendRequestFileChunk
    }
}
