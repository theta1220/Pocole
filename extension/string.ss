extension string
{
    func to_array()
    {
        var arr = [];
        system_call("Sumi.Lib.String.ToArray", arr, this);
        return arr;
    }

    test to_array()
    {
        var text = "hoge";
        var arr = text.to_array();
        if (arr.len() == 4)
        {
            return true;
        }
        return false;
    }

    func split(sep)
    {
        var res = [];
        var buf = "";
        foreach(c : this.to_array())
        {
            if(c == " ")
            {
                res.push(buf);
                buf = "";
                continue;
            }
            buf = buf + c;
        }
        if(buf != "")
        {
            res.push(buf);
        }
        return res;
    }

    test split()
    {
        var text = "hoge foo bar";
        var arr = text.split(" ");

        if(arr[0] != "hoge")
        {
            return false;
        }
        if(arr[1] != "foo")
        {
            return false;
        }
        if(arr[2] != "bar")
        {
            return false;
        }
        return true;
    }
}

class string
{
    func format(text, args)
    {
        var res = "";
        system_call("Sumi.Lib.String.Format", res, text, args);
        return res;
    }
}