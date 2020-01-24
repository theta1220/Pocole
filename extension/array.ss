extension array
{
    func push(item)
    {
        system_call("Sumi.Lib.Array.Push", null, this, item);
    }

    func pop() : object
    {
        if(this.len() == 0)
        {
            return null;
        }
        var item = this.last();
        system_call("Sumi.Lib.Array.RemoveAt", null, this, this.len() - 1);
        return item;
    }

    func len() : int
    {
        var res = 0;
        system_call("Sumi.Lib.Array.Len", res, this);
        return res;
    }

    func first() : object
    {
        if(this.len() > 0)
        {
            return this[0];
        }
        return null;
    }

    func last() : object
    {
        if(this.Len() > 0)
        {
            return this[this.len()-1];
        }
        return null;
    }

    func remove(item)
    {
        system_call("Sumi.Lib.Array.Remove", null, this, item);
    }
}