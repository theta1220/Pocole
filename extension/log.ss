
class log
{
    func info(text, args)
    {
        system_call("Sumi.Log.SumiPrint", null, "info", text, args);
    }

    func debug(text, args)
    {
        system_call("Sumi.Log.SumiPrint", null, "debug", text, args);
    }

    func warn(text, args)
    {
        system_call("Sumi.Log.SumiPrint", null, "warn", text, args);
    }

    func error(text, args)
    {
        system_call("Sumi.Log.SumiError", null, text, args);
    }


    func no_impl()
    {
        system_call("Sumi.Log.SumiError", null, "未実装です");
    }

    func assert(cond, text, args)
    {
        if(cond)
        {
            return;
        }
        system_call("Sumi.Log.SumiError", null, text, args);
    }
}