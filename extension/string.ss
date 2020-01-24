
extension string
{
    func split(sep)
    {
        foreach(c : sep)
        {
            
        }
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