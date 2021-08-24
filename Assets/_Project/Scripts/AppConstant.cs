using UnityEngine;
using System.Collections;

public class AppDevelopFlag
{
	public static readonly bool	DEVELOP_SYSTEM = true;
    public static readonly bool	DEVELOP_USER   = true;
    public static readonly bool	RELEASE        = false;
}

public class AppPaths
{
	public static readonly string  	PERSISTENT_DATA      = Application.persistentDataPath;
	public static readonly string	PATH_RESOURCE_SFX    = "Sounds/MenuSfx/";
    public static readonly string	PATH_RESOURCE_MUSIC  = "Sounds/Music/";
}

public class AppScenes
{
	public static readonly string 	MAIN_SCENE    = "Menu";
	public static readonly string 	LOADING_SCENE = "Loading";
	public static readonly string 	GAME_SCENE    = "Game";
}

public class AppPlayerPrefKeys
{
	public static readonly string	MUSIC_VOLUME = "MusicVolume";
	public static readonly string	SFX_VOLUME   = "SfxVolume";
}

public class AppSounds
{
	public static readonly string	MAIN_TITLE_MUSIC = "MainTitle";
	public static readonly string	GAME_MUSIC       = "Gameplay";
    public static readonly string	BUTTON_SFX       = "Click_Soft_01";
}



