
extension object
{
    func has_value(name) : bool
    {
        var res = false;
        system_call("Sumi.Class.HasValue", res, this, name);
        return res;
    }

    func has_func(name) : bool
    {
        var res = false;
        system_call("Sumi.Class.HasFunction", res, this, name);
        return res;
    }
}

class object
{
    func new()
    {
        return @this;
    }
}