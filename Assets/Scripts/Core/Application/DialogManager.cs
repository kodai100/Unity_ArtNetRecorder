using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class DialogManager : SingletonMonoBehaviour<DialogManager>
{

    [SerializeField] private DialogUI dialogUiPrefab;

    [SerializeField] private RectTransform dialogParentTransform;

    public static async UniTask<bool> OpenInfo(string message)
    {
        var instance = InstantiateDialog();
        return await instance.OpenInfo(message);
    }

    public static async UniTask<bool> OpenError(string message)
    {
        var instance = InstantiateDialog();
        return await instance.OpenError(message);
    }

    private static DialogUI InstantiateDialog()
    {
        var instance = Instantiate(Instance.dialogUiPrefab, Instance.dialogParentTransform);
        return instance;
    }

}
