# ExtendableTargeting

## なにこれ?

拡張可能なターゲティングを提供するモジュールです。

以下のようなターゲティングを提供します。

- `@prefix` - パラメータなしのターゲット
- `@prefix=value` - パラメータありのターゲット

## 組み込みターゲット

以下のターゲティングが組み込みで提供されます。

| ターゲット文字列        | 説明                         | 例        |
|-----------------|----------------------------|----------|
| `@me`           | 自分自身をターゲットにします。            | `@me`    |
| `@!me`          | 自分以外をターゲットにします。            | `@!me`   |
| `@aim`          | エイム先のプレイヤーをターゲットにします。(WIP) | `@aim`   |
| `@ct`           | CTチームのプレイヤーをターゲットにします。     | `@ct`    |
| `@t`            | Tチームのプレイヤーをターゲットにします。      | `@t`     |
| `@spec`         | 観戦者をターゲットにします。             | `@spec`  |
| `@bot`          | BOTプレイヤーをターゲットにします。        | `@bot`   |
| `@human`        | 人間プレイヤーをターゲットにします。         | `@human` |
| `@alive`        | 生存しているプレイヤーをターゲットにします。     | `@alive` |
| `@dead`         | 死亡しているプレイヤーをターゲットにします。     | `@dead`  |
| `#<数値>`         | SteamIDまたはUserIDでターゲットします。 | `#12345` |


## 使い方

### 依存関係

NuGetから`TnmsExtendableTargeting.Shared`をインストールします。

TODO

```xml
```

### 使用例

以下のようにして、`IExtendableTargeting`を取得します。

```csharp

private IExtendableTargeting _extendableTargeting = null!;


public void OnAllModulesLoaded()
{
    var extendableTargeting = _sharedSystem.GetSharpModuleManager()
        .GetRequiredSharpModuleInterface<IExtendableTargeting>(IExtendableTargeting.ModSharpModuleIdentity).Instance;
    _extendableTargeting = extendableTargeting ?? throw new InvalidOperationException("TnmsExtendableTargeting is not found! Make sure TnmsExtendableTargeting is installed!");
}
```

そして、以下のようにしてターゲティングを解決します。

```csharp
if(_extendableTargeting.ResolveTarget(targetString, player, out var foundTargets)
{
    // Do something with foundTargets
}
```


## カスタムターゲットの作成

ExtendableTargetingでは名前の通り、カスタムターゲットを追加し拡張することが出来ます。

追加したターゲットは、`IExtendableTargeting.ResolveTarget` を使用する全ての場所で使用可能になります。 すなわち、これらを使用する全てのmoduleで使用可能になります。

### パラメータなしのカスタムターゲット

パラメータなしのカスタムターゲットは、`IExtendableTargeting.RegisterCustomTarget` を使用して登録します。

Prefixは必ず`@`で始まる必要があり、始まっていない場合は強制的に`@`が付与されます。

例:
- `@mytarget` -> `@mytarget`
- `mytarget` -> `@mytarget`
- `$mytarget` -> `@$mytarget`

```csharp
IExtendableTargeting.RegisterCustomTarget("@me", Me);

public static bool Me(IGameClient targetClient, IGameClient? caller)
{
    return caller?.Slot == targetClient.Slot;
}
```

### パラメータありのカスタムターゲット

パラメータありのカスタムターゲットは、`IExtendableTargeting.RegisterCustomTargetWithParam` を使用して登録します。

Prefixは必ず`@`で始まる必要があり、始まっていない場合は強制的に`@`が付与されます。

例:
- `@mytarget` -> `@mytarget`
- `mytarget` -> `@mytarget`
- `$mytarget` -> `@$mytarget`

```csharp
IExtendableTargeting.RegisterCustomTargetWithParam("@slot", Slot);
public static bool Slot(string param, IGameClient targetClient, IGameClient? caller)
{
    if(int.TryParse(param, out var slot))
    {
        return targetClient.Slot == slot;
    }
    return false;
}
```

パラメータ有りのカスタムターゲットは、`@prefix=value`の形式で使用可能です。

