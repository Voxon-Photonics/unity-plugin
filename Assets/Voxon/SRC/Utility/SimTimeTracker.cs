using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxon;

[RequireComponent(typeof(Voxon.VXTextComponent))]
public class SimTimeTracker : MonoBehaviour, IDrawable
{
	double TimePassed = 0;
	public bool showOnVolume = true;
	public bool showOnTouchScreen = false;
	Voxon.VXTextComponent text;
	public int textScreenPosX = 20;
	public int textScreenPosY = 550;


	int minutes = 0;
	int seconds = 0;

	// Start is called before the first frame update
	void Start()
    {
		text = GetComponent<Voxon.VXTextComponent>();
		if (showOnTouchScreen)
        {
			VXProcess.Drawables.Add(this);
		}
    }

	// Update is called once per frame
	void Update()
	{
		TimePassed += Time.deltaTime;
		minutes = (int)(TimePassed / 60);
		seconds = (int)(TimePassed % 60);

		if (showOnVolume) { 
		
			text.SetString($"SimTime: {minutes.ToString("D2")}:{seconds.ToString("D2")}");
		}
		if (showOnTouchScreen)
        {
			Draw();
        }

	}

    public void Draw()
    {
		int textCol = 0xffff00;
		Voxon.VXProcess.Runtime.LogToScreenExt(textScreenPosX, textScreenPosY, textCol, -1, $"SimTime: {minutes.ToString("D2")}:{seconds.ToString("D2")}");
    }
}
