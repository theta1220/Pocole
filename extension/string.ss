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
            if(c == sep)
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

    func split_once(sep)
    {
        var res = [];
        var buf = "";
        var read_num = 1;

        foreach(c : this.to_array())
        {
            if((read_num > res.len()) && (c == sep))
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

    test split_once()
    {
        var text = "hoge foo bar";
        var arr = text.split_once(" ");

        if(arr[0] != "hoge")
        {
            return false;
        }
        if(arr[1] != "foo bar")
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