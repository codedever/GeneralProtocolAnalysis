
namespace GeneralProtocolAnalysis
{
    /// <summary>
    /// 一种类型到中间类型的相互转化
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TTarget"></typeparam>
    public interface IConverter<TSource, TTarget>
    {
        TTarget Encode(TSource source);
        TSource Decode(TTarget target);
    }
}
