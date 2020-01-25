## 概要
sumiとは、C#上で動作するインタプリタ言語です。
オブジェクト指向です。

## 使い方

C#に組み込むことが前提であるため、
sumiの実行にはC#でsumiのインスタンスを生成する必要があります。
```C#
var script = Sumi.Loader("out.so");
script.ForceExecute();
```

sumiを１ファイルにまとめてる `sumi_build` を実行し、
dotnetやUnityから、sumiを呼び出します。
```bash
sumi_build out.so
dotnet run
```


## 言語仕様

### 変数
`var` キーワードを使用して、変数を定義できます。
型を特定するため、変数は必ず初期化する必要があります。
```C#
var a = 0;
var text = "hello.";
var boolean = false;
var func_result = foo.bar();
var class_instance = hoge.new();
```

#### サポートされている型
* int
* string
* bool
* class
* array

### 関数
`func` キーワードを使用して、関数を定義できます。
クラスのメンバである必要はありません。
```C#
func add(a, b) : int
{
    return a + b;
}
```

### クラス
`class` キーワードを使用して、クラスを定義できます。
クラスの内部に宣言した変数と関数は、インスタンスに保持されます。

```C#
class hoge : object
{
    var num = 999;
    func print()
    {
        ...
    }
}
```

### テスト関数
`test` キーワードを使用して、テスト関数を定義できます。
テスト関数とは、主に関数をテストする目的で使用されます。
テスト関数は、static関数であるため、実体を持ちません。
```C#
func add(a, b)
{
    return a + b;
}

test add()
{
    var result = add(10, 20);
    if(resutl == 30)
    {
        return true;
    }
    return false;
}
```