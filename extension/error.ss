
class error
{
    func log(text)
    {
        system_call("Sumi.Lib.Debug.Error", null, text, args);
    }

    func format(text, args)
    {
        log(string.format(text, args));
    }

    func no_impl()
    {
        log("未実装の関数です");
}
}