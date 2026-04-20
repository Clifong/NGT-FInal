public class ChoiceDatabase: AddressableDatabase<ChoiceDatabase>
{
    protected override void Awake()
    {
        SetLabel("Choice");
        base.Awake();
    }
}