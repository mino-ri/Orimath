namespace Orimath.UITest

type UITestPluginSetting =
    { mutable ContentText: string }

type TestData =
    { mutable Id: int
      mutable Value: string
      Children: TestData[] }
    with
    override this.ToString() = this.Id.ToString() + ":" + this.Value
