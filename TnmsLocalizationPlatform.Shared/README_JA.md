# TnmsLocalizationPlatform

## なにこれ?

プレイヤーの言語設定を管理し、各プラグインが簡単に多言語対応出来るようにするための共通基盤です。

初回接続時は、プレイヤーの`cl_language`設定を自動的に取得・保存します。

## 機能

### プレイヤー言語設定の自動管理

プレイヤーがサーバーに接続すると、自動的に`cl_language`設定を取得し、データベースに保存します。次回接続時には保存された言語設定を使用します。

### 多言語対応のLocalizerの提供

`ITnmsLocalizer`インターフェースを通じて、Microsoft.Extensions.Localizationベースの多言語対応機能を提供します。

## 使い方

### 依存関係の追加

NuGetから`TnmsLocalizationPlatform.Shared`をインストールします。

TODO

```xml
```

### プラグイン開発

#### インスタンスの取得

```csharp
private ITnmsLocalizationPlatform? _localizationPlatform;

public void OnAllModulesLoaded()
{
    _localizationPlatform = SharedSystem.GetSharpModuleManager()
        .GetRequiredSharpModuleInterface<ITnmsLocalizationPlatform>(
            ITnmsLocalizationPlatform.ModSharpModuleIdentity).Instance;
}
```

#### ILocalizableModuleの実装

あなたのプラグインで`ILocalizableModule`を実装します。

```csharp
public class YourPlugin : IModSharpModule, ILocalizableModule
{
    public string ModuleDirectory => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
    
    // ...その他の実装
}
```

#### Localizerの作成と使用

```csharp
private ITnmsLocalizer _localizer = null!;

// Localizerの作成
public void OnAllModulesLoaded()
{
    _localizer = _localizationPlatform.CreateStringLocalizer(this);
}

// 使用例
public void SendLocalizedMessage(IGameClient client, string key, params object[] args)
{
    var localizedString = _localizer.ForClient(client, key, args);
    client.PrintToChat(localizedString.Value);
}

// 特定の文化圏での文字列取得
public string GetLocalizedString(string key, CultureInfo culture, params object[] args)
{
    if (_localizer != null)
    {
        return _localizer[key, culture, args].Value;
    }
    return key;
}
```

#### プレイヤーの言語設定取得

```csharp
public void CheckPlayerLanguage(IGameClient client)
{
    var playerCulture = _localizer.GetClientCulture(client);
    Console.WriteLine($"Player {client.PlayerName} uses language: {playerCulture.Name}");
}
```

### 言語ファイルの配置

modules/yourmodle/lang/

## 依存関係

- `TnmsCentralizedDbPlatform`: データベース接続とユーザー言語設定の永続化
- `Microsoft.Extensions.Localization`: 多言語対応の基盤

詳細はコードドキュメントを確認してください。
