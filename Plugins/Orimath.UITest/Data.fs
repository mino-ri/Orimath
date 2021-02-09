namespace Orimath.UITest

type UITestPluginSetting() =
    member val ContentText = "Content" with get, set

type TestData =
    { mutable Id: int
      mutable Value: string
      Children: TestData[] }
    with
    override this.ToString() = this.Id.ToString() + ":" + this.Value
