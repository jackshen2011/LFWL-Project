using UnityEngine;
using System.Collections;

class QinRenBiSaiPaiHangUI : ListUiDtCtrl
{
    public override void ShowListUiDt(object[] args)
    {
        UiText[0].text = "1"; //排名.
        UiText[1].text = "qrmj"; //昵称.
        UiText[2].text = "100"; //积分.
        if (MainRoot._gPlayerData.pAsyncImageDownload != null)
        {
            MainRoot._gPlayerData.pAsyncImageDownload.LoadingUrlImage("", UiImg[0]); //玩家头像.
        }
    }
}