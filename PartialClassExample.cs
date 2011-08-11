using UnityEngine;
using System.Collections;


public partial class BigClass : MonoBehaviour
{
	private bool inFirstFile;
	
	
	private void firstFile()
	{
		this.inSecondFile = false;
	}
}


// contents of BigClass2.cs
public partial class BigClass : MonoBehaviour
{
	private bool inSecondFile;
	
	
	private void secondFile()
	{
		this.inFirstFile = true;
	}
}