public interface ICollectionObjectOperatoredOn: IObjectOperatedOn
{
    public bool Contains(IObjectOperatedOn other);
    public bool NotContains(IObjectOperatedOn other);
}

