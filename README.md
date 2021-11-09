## 概要
sumiとは、C#上で動作するインタプリタ言語です。
オブジェクト指向です。

## 使い方

### サンプル
`main.ss`
sumiソースファイルの拡張子は `ss` (sumi script)を指定してください
```C#
class main : object
{
    func hello()
    {
        log.info("hello sumi.");
    }
}

var _main = main.new();
_main.hello();
```
```bash
hello sumi.
```

### 特徴
sumiはメソッドとして、どこにでもテスト関数を記載することができます。
sumiの実行時にテスト関数がすべて実行されます。
```
test hoge()
{
    ...
    return true;
}
```

### 組み込み方法
sumiを１ファイルにまとめる `sumi_build` を実行しておきます。
```bash
sumi_build out.so
```
※sumi_build ファイルは現在公開しておりません

C#に組み込むことが前提であるため、
sumiの実行にはC#でsumiのインスタンスを生成する必要があります。
```C#
var script = Sumi.Loader("out.so");
script.ForceExecute();
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
var array = [123,456,789];
```

### 配列
```C#
// 空でも宣言可能です。
var array = [];

// pushで末尾に値を追加できます。
array.push(123);
array.push(456);

// popで末尾の値を取り出すことができます
var res = array.pop();

// firstで先頭の値にアクセスできます
var res = array.first();

// lastで末尾の値にアクセスできます
var res = array.last();

// クラスインスタンスを扱うこともできます。
var class_arr = [hoge.new(), hoge.new()];
array.push(hoge.new());
```

#### 変数としてサポートされている型
* int
* string
* bool
* class
* array

### 処理文
#### if
```C#
if (hoge == false)
{
    ...
}
else if ((foo == true) && (hoge == true))
{
    ...
}
else
{
    ...
}
```

#### for
```C#
var arr = [123, 456, 789];
for(var i=0; i<arr.Len(); i=i+1)
{
    log.info("arr:{0}/{1}", i, arr[i]);
}
```

#### foreach
`:` で仕切られた順に `変数名` `配列名` `インデックス変数名` を指定します。
変数及び、インデックス変数には、事前に宣言する必要はありません。
```C#
var arr = [123, 456, 789];
foreach(item : arr : i)
{
    log.info("arr:{0}/{1}", i, item.num);
}
```

#### while
```C#
var arr = [123, 456, 789];
var cnt = 0;
while(cnt < arr.Len())
{
    log,info("arr:{0}/{1}", cnt, arr[cnt]);
    cnt = cnt + 1;
}
```

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
`object` を継承すると、 `new()` が使用できるようになり、
クラスをインスタンス化させることができます。
また、他のクラスを継承することも可能です。
```C#
class hoge : object
{
    var num = 999;
    func print()
    {
        ...
    }
}

class foo : hoge
{
    ...
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
