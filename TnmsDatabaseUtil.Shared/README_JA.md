# TnmsCentralizedDbPlatform

## なにこれ?

DBの依存関係を統一するためのモジュールです。

このモジュールでは、Entity Framework Core (EF Core)のDB接続部分のみを共通化し、複数のEF Coreの依存関係が混在することを防ぎます。

## 使い方

### 依存関係の追加

NuGetから`TnmsAdministrationPlatform.Shared`をインストールします。

TODO

```xml
```

### 実際の実装例

[LocalizetionPlatform](../TnmsLocalizationPlatform)を参照してください。 多分それが一番早いです。

### モデルとDbContextの定義やMigration等の作成

このモジュールは、依存関係の混在を防ぐための物のため、実装は通常のEF Coreの実装が必要です。

EF Coreのやり方に従い、モデルとDbContextを定義、Migrationを作成します。

やり方は各自調べてください。

### インスタンスの取得

```csharp


private ITnmsCentralizedDbPlatform? _dbPlatform;

public void OnAllModulesLoaded()
{
    _dbPlatform = SharedSystem.GetSharpModuleManager()
        .GetRequiredSharpModuleInterface<ITnmsCentralizedDbPlatform>(
            ITnmsCentralizedDbPlatform.ModSharpModuleIdentity).Instance;
}
```

### DbParamを作成

細かいパラメータは、[DbConnectionParameters.cs](DbConnectionParameters.cs) を参照してください。

以下はSqliteの例です。

```csharp
var dbParams = new DbConnectionParameters
{
    ProviderType = TnmsDatabaseProviderType.Sqlite,
    Host = "localization.db"
};
```

###  DbContextOptionsBuilderを作成する

第一引数は、先程作成した`DbConnectionParameters`です。

第ニ引数は、あなたのモジュール名 (アセンブリ名) を指定してください。

```csharp
var options = _dbPlatform.ConfigureDbContext<LocalizationDbContext>(dbParams, "TnmsLocalizationPlatform");
```

### 後はEF Coreのやり方に従ってください。

残りは、通常のEF Coreのやり方に従ってください。
