namespace WhiteEagles.Infrastructure.TransientFaultHandling
{
    public class TransientDefaultDetectionStrategy<T> 
        : TransientFaultDetectionStrategy<T>
    {
        public override bool IsTransientResult(T result) => Equals(result, default(T));
    }
}
