namespace AdminClient.Networking
{
    public enum OutgoingPacket
    {
        SendRegisterUser,
        SendLoginUser,
        SendRequestLeaveGame,
        SendRequestFileChunk,
        SendFileMeta,
        SendFileChunk,
        SendEndSendingFile
    }
}
