public interface INumberOperatedOn : IObjectOperatedOn
{
    public bool Equals(INumberOperatedOn other);
    public bool NotEquals(INumberOperatedOn other);
    public bool MoreThan(INumberOperatedOn other);
    public bool MoreThanEqualTo(INumberOperatedOn other);
    public bool LessThanQualTo(INumberOperatedOn other);
    public bool LessThan(INumberOperatedOn other);
}
