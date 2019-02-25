using System;
using System.Collections.Generic;
using System.Text;

namespace GeneralProtocolAnalysis
{
    /// <summary>
    /// 一种类型到中间类型的相互转化
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TTarget"></typeparam>
    public interface IConverter<TSource, TTarget>
    {
        Protocol Protocol { get; }
        TTarget Encode(TSource source, string message = null);
        TSource Decode(TTarget target, string message = null);
    }
}
