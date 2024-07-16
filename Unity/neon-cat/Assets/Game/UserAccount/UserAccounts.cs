using GameLib.Alg;
using UnityEngine;

[ScriptExecutionOrder(-9)]
public class UserAccounts : Singleton<UserAccounts>
{
    private UserAccount[] _accounts;
    private string[] _accountNames;
    public bool DeleteAccountsOnAwake;

    protected override void Awake()
    {
        base.Awake();
        _accounts = GetComponentsInChildren<UserAccount>();
        if(DeleteAccountsOnAwake)
            PlayerPrefs.DeleteAll();
        LoadAccounts();
    }
    private void LoadAccounts()
    {
        _accounts[0].Load();
    }

    public UserAccount GetCurrentAccount()
    {
        return _accounts[0];
    }
}
