
func log(text, args)
{
    system_call("Pot.Sumi.Log", null, text, args);
}

func assert(condition, text, args)
{
    if(condition == false)
    {
        error.format(text, args);
    }
}