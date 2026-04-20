
public interface IOnlyEqualityObjectOperatedOn : IObjectOperatedOn
{
    public bool Equals(IOnlyEqualityObjectOperatedOn other);
    public bool NotEquals(IOnlyEqualityObjectOperatedOn other);
}
