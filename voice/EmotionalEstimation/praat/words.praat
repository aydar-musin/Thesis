#
# Word chomper: This script marks the boundaries between individual
# utterances based on the intensity (dB level) at each time.
# Author: Joseph Toscano <joseph-toscano@uiowa.edu>
# updated 11-Jan-2008: bug fix from Neil Bardhan.
#

# Preferences:
form arguments
	sentence File
endform


	clearinfo
	#file$="C:\praatfiles\samples\happiness\03a04Fd.wav"
	
	Read from file... 'file$'
	current_sound$ = selected$("Sound")+"_denoised"
	Remove noise... 0.0 0.0 0.025 80.0 10000.0 30.0 Spectral subtraction
	Scale intensity... 70.0
	
	#current_sound$ = current_sound$+'_denoised'

	select Sound 'current_sound$'
		
		Resample... 11025 50
		To Pitch... 0.01 60 1000
		
		select Sound 'current_sound$'_11025
		To Intensity... 100 0
		
		select Sound 'current_sound$'
		fTime = Get finishing time
		numTimes = fTime / 0.01
		
		values$=""
		for itime to numTimes
			select Pitch 'current_sound$'_11025
			curtime = 0.01 * itime
			f0 = 0
			f0 = Get value at time... 'curtime' Hertz Linear
			f0$ = fixed$ (f0, 2)

			if f0$ = "--undefined--"
				f0$ = "0"
			endif

			
			curtime$ = fixed$ (curtime, 5)
		 
			select Intensity 'current_sound$'_11025
			intensity = Get value at time... 'curtime' Cubic
		
		
			intensity$ = fixed$ (intensity, 2)
			if intensity$ = "--undefined--"
				intensity$ = "0"
			endif

			values$=values$+f0$+"/"+intensity$+" "
		endfor
		writeInfoLine: values$