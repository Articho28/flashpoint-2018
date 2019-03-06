using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToConnectionScreen : MonoBehaviour
{
    public int numPlayers;

    public void OnOnePlayerClicked()
    {
        numPlayers = 1;
        switchScenes();
    }
    public void OnTwoPlayerClicked()
    {
        numPlayers = 2;
        switchScenes();
    }
    public void OnThreePlayerClicked()
    {
        numPlayers = 3;
        switchScenes();
    }
    public void OnFourPlayerClicked()
    {
        numPlayers = 4;
        switchScenes();
    }
    public void OnFivePlayerClicked()
    {
        numPlayers = 5;
        switchScenes();
    }
    public void OnSixPlayerClicked()
    {
        numPlayers = 6;
        switchScenes();
    }

    public void switchScenes()
    {
        MultiplayerSetting.MS.setFamilyGame(true);
        MultiplayerSetting.MS.setNumberOfPlayers(numPlayers);
        SceneManager.LoadScene("ServerConnection", LoadSceneMode.Single);
    }
}
