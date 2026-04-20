public class RememberChoiceDatabase: AddressableDatabase<RememberChoiceDatabase>
{
    protected override void Awake()
    {
        SetLabel("Remember");
        base.Awake();
    }
}
