namespace GeneralProtocolAnalysis
{
    /// <summary>
    /// 一种类型到中间类型的相互转化
    /// </summary>
    /// <typeparam name="Message"></typeparam>
    /// <typeparam name="TTarget"></typeparam>
    public interface IMessageConverter<TTarget> : IConverter<Message, TTarget>
    {
        Protocol Protocol { get; }
        Message Decode(TTarget target, string messageName);
    }
}
