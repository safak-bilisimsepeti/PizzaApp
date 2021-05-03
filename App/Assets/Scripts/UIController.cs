using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [Header("Line Settings")]
    Vector3 startPosition;
    GameObject currentLineObject;
    LineRenderer currentLineRenderer;
    public Material lineMaterial;
    public float lineThickness;

    [Header("Data")]
    public int selectedIcon = 0;
    public Team[] teams;
    public Sprite[] teamIcons;
    public Canvas parentCanvas;
    public GameObject mainMenu;
    public Transform createTeamPanel;
    public UnityEngine.UI.Slider stars;
    public UnityEngine.UI.Text shoots;
    public UnityEngine.UI.ScrollRect leaderboard;
    public UnityEngine.UI.ScrollRect rewards;
    public UnityEngine.UI.ScrollRect icons;
    public UnityEngine.UI.ScrollRect members;
    public UnityEngine.UI.Text score;
    public UnityEngine.UI.InputField invitePhone;
    public GameObject sliceReward;
    public GameObject LBTeamEntry;
    public GameObject RewardEntry;
    public GameObject iconEntry;

    // Start is called before the first frame update
    void Start()
    {
        RefreshUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GetComponent<PlayerController>().play)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            StartDrawingLine();
        }
        else if (Input.GetMouseButton(0))
        {
            PreviewLine();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            StartCoroutine(EndDrawingLine());
        }
    }

    Vector3 GetMousePosition()
    {
        Vector2 movePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.transform as RectTransform,
            Input.mousePosition, parentCanvas.worldCamera,
            out movePos);
        Vector3 positionToReturn = parentCanvas.transform.TransformPoint(movePos);
        positionToReturn.z = parentCanvas.transform.position.z - 0.01f;
        return positionToReturn;
    }

    void StartDrawingLine()
    {
        if (!GetComponent<PlayerController>().play)
            return;

        startPosition = GetMousePosition();
        currentLineObject = new GameObject();
        currentLineObject.transform.position = startPosition;
        currentLineRenderer = currentLineObject.AddComponent<LineRenderer>();
        currentLineRenderer.material = lineMaterial;
        currentLineRenderer.startWidth = lineThickness;
        currentLineRenderer.endWidth = lineThickness / 5;

        Gradient gradient = new Gradient();
        GradientAlphaKey key = new GradientAlphaKey();
        key.alpha = 0.5f;
        gradient.alphaKeys = new GradientAlphaKey[] { key, key };
        currentLineRenderer.colorGradient = gradient;
        
        currentLineRenderer.positionCount = 1;
        currentLineRenderer.SetPosition(0, startPosition);
    }

    void PreviewLine()
    {
        Vector3 lastPosition = GetMousePosition();
        currentLineRenderer.positionCount++;
        currentLineRenderer.SetPosition(currentLineRenderer.positionCount - 1, lastPosition);
        startPosition = lastPosition;
        currentLineRenderer.Simplify(0.01f);
    }

    IEnumerator EndDrawingLine()
    {
        yield return new WaitForSeconds(0.0f);

        if ( currentLineRenderer != null && currentLineRenderer.positionCount > 1)
        {
            GetComponent<PlayerController>().Kick(currentLineRenderer);

            Destroy(currentLineObject);
            startPosition = Vector3.zero;
            currentLineObject = null;
            currentLineRenderer = null;
        }
        
    }

    public void RefreshUI()
    {
        int star = Local.user.CalculateStar(Local.user.ratingScore);
        stars.value = star * 0.2f;

        int current = GetComponent<PlayerController>().token == null ? 0 : 1;
        shoots.text = (Local.user.gameData[GetComponent<PlayerController>().multiplayer ? 2 : 1].tokens.Count + current).ToString() + " / 10";

        score.text = Local.user.gameData[1].points.ToString();
        int slices = Local.user.gameData[1].points / 1000;
        sliceReward.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = slices.ToString();
        sliceReward.SetActive(slices > 0);
    }

    public void ShowLeaderboard(Team[] teams)
    {
        Camera.main.GetComponent<Core>().ClearContent(leaderboard);

        for (int i = 0; i < teams.Length; i++)
        {
            Team team = teams[i];
            GameObject t = Instantiate(LBTeamEntry);
            t.transform.SetParent(leaderboard.content);
            t.transform.localPosition = Vector3.zero;
            t.transform.rotation = Quaternion.identity;
            t.transform.localScale = Vector3.one;

            t.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = i.ToString();
            t.transform.GetChild(1).GetComponent<UnityEngine.UI.Image>().sprite = teamIcons[team.logoIndex];
            t.transform.GetChild(2).GetComponent<UnityEngine.UI.Text>().text = team.name;
            t.transform.GetChild(3).GetComponent<UnityEngine.UI.Text>().text = team.GetTotalScore().ToString();
            t.GetComponent<LBTeamEntry>().index = i;
        }
    }

    public void ShowRewards(Team team)
    {
        Camera.main.GetComponent<Core>().ClearContent(rewards);

        for (int i = 0; i < team.members.Length; i++)
        {
            TeamMember member = team.members[i];
            GameObject t = Instantiate(LBTeamEntry);
            t.transform.SetParent(leaderboard.content);
            t.transform.localPosition = Vector3.zero;
            t.transform.rotation = Quaternion.identity;
            t.transform.localScale = Vector3.one;

            t.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = teamIcons[team.logoIndex];
            t.transform.GetChild(1).GetComponent<UnityEngine.UI.Text>().text = member.name;
            t.transform.GetChild(2).GetComponent<UnityEngine.UI.Text>().text = member.score.ToString();
            t.transform.GetChild(3).GetComponent<UnityEngine.UI.Text>().text = member.slices.ToString();
        }
    }

    public void SelectTeamRewards(int index)
    {
        ShowRewards(teams[index]);
    }

    public void ShowTeamIcons()
    {
        Camera.main.GetComponent<Core>().ClearContent(icons);

        for (int i = 0; i < teamIcons.Length; i++)
        {
            Sprite icon = teamIcons[i];
            GameObject t = Instantiate(iconEntry);
            t.transform.SetParent(icons.content);
            t.transform.localPosition = Vector3.zero;
            t.transform.rotation = Quaternion.identity;
            t.transform.localScale = Vector3.one;
            t.GetComponent<UnityEngine.UI.Image>().sprite = icon;
        }
    }

    public void SelectTeamIcon(int index)
    {
        selectedIcon = index;
        createTeamPanel.GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = teamIcons[selectedIcon];
    }

    public void CreateTeam()
    {
        Team team = new Team();
        team.name = createTeamPanel.GetChild(1).GetComponent<UnityEngine.UI.InputField>().text;
        team.logoIndex = selectedIcon;
        team.members = new TeamMember[] { new TeamMember(Local.user.phone, Local.user.name + " " + Local.user.surname) };
        // Send to BE
    }

    public void InviteUser()
    {
        string phone = invitePhone.text;
        invitePhone.text = "";

    }

    public void ConvertToSlices()
    {
        int slices = Local.user.gameData[1].points / 1000;
        GameObject w = Instantiate(Resources.Load<GameObject>("GiftPanel"));
        w.GetComponent<Gift>().Init(slices);

        Local.user.gameData[1].points -= slices * 1000;
        RefreshUI();
    }

    public void ShowMainMenu()
    {
        GetComponent<PlayerController>().play = false;
        mainMenu.SetActive(true);
    }
}
