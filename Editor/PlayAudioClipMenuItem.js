
static private var audioPlayer : AudioSource; 

// Create a menu item with a short cut of cmd + u
@MenuItem("Custom/Play Audio Clip %u")
static function PlayAudioClip()
{
   // First create the audio source (if not already).
   if( !audioPlayer )
   { 
      audioPlayer = GameObject ("Editor Audio Player", AudioSource).audio; 
      audioPlayer.gameObject.hideFlags = HideFlags.HideAndDontSave;
   }

   // Interrupt the current clip, then play the new one.
   if( audioPlayer.isPlaying )
      audioPlayer.Stop();

   // Get all selected Audio Clips.
   var selectedClips = Selection.GetFiltered( AudioClip, SelectionMode.Assets );

   // If there was one audio clip selected, then play it. 
   if( selectedClips.Length == 1 )
   {
      audioPlayer.clip = selectedClips [0] as AudioClip;
      audioPlayer.Play();
   }
}