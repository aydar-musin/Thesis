#
# Word chomper: This script marks the boundaries between individual
# utterances based on the intensity (dB level) at each time.
# Author: Joseph Toscano <joseph-toscano@uiowa.edu>
# updated 11-Jan-2008: bug fix from Neil Bardhan.
#

# Preferences:
form arguments
	positive dblevel 50
	positive tiernum  1
	word File C:\Users\Айдар\SkyDrive\Учеба\innopolis\Thesis\voice\voice\praat\files\Anger.wav
	positive spacing_between_words(s) 0.3
endform


	clearinfo

	move_cursor = spacing_between_words / 3
	
	Read from file... 'file$'
	current_sound$ = selected$("Sound")+"_denoised"
	Remove noise... 0.0 0.0 0.025 80.0 10000.0 30.0 Spectral subtraction
	Scale intensity... 70.0
	
	#current_sound$ = current_sound$+'_denoised'
	select Sound 'current_sound$'

	# First, we should select the sound and create a textgrid.
	select Sound 'current_sound$'
	To TextGrid... wordseg wordseg
	select Sound 'current_sound$'

	# Store start and finish times.
	start=Get starting time
	finish=Get finishing time

	# Edit the sound.
	select Sound 'current_sound$'
	plus TextGrid 'current_sound$'
	Edit
	
	# In order to analyze the intensity, we must be viewing at most 10 seconds of time. So, we must
	# move the window in 10 second increments until we reach the end.
	
	# Get into the editor
	editor TextGrid 'current_sound$'
	
		# The bet_words variable counts the number of consecutive jumps we make between words.
		bet_words = 1
	
		Select... 0 0
		Add interval on tier 1
		Move cursor to... 0.1
		Show analyses... yes no yes no no 10
	
		# Segment the words.
		i = 0
		repeat
			Zoom... i i+10
	
			repeat
				int = Get intensity
				if int <> undefined and int < dblevel
					if bet_words = 3
						bet_words = bet_words + 1
						Add interval on tier 1
					else
						bet_words = bet_words + 1
					endif
				else
					bet_words = 0
				endif
				
				Move cursor by... 'move_cursor'
				cursor = Get cursor
				if cursor >= finish
					cursor = i + 10
				endif

			until cursor >= i+10

			i = i + 10
			cursor = Get cursor
			
		until cursor >= finish
	
	endeditor


	select TextGrid 'current_sound$'
	numpoints = Get number of points... 1

	select Sound 'current_sound$'
	Edit

	for i from 2 to numpoints-1

		select TextGrid 'current_sound$'
		start = Get time of point... 1 i
		finish_time = Get finishing time
		if i = numpoints
			finish = finish_time
		else
			finish = Get time of point... 1 'i'+1
		endif

		editor Sound 'current_sound$'

			Select... 'start' 'finish'
                	Extract selection

        	endeditor
		

        	select Sound untitled
	endfor
	endeditor
	
	for i from 1 to numpoints-2

		select Sound untitled
	
		newname$="word_'i'"
		Rename... 'newname$'
		
		Resample... 11025 50
		To Pitch... 0.01 60 1000
		Rename... 'newname$'

		select Sound word_'i'_11025
		To Intensity... 100 0
		Rename... 'newname$'
		
		select Sound 'newname$'
		fTime = Get finishing time
      	# Use numTimes in the loop
		numTimes = fTime / 0.01

		values$=""

		for itime to numTimes
			select Pitch 'newname$'
			curtime = 0.01 * itime
			f0 = 0
			f0 = Get value at time... 'curtime' Hertz Linear
			f0$ = fixed$ (f0, 2)

			if f0$ = "--undefined--"
				f0$ = "0"
			endif

			
			curtime$ = fixed$ (curtime, 5)
		 
			select Intensity word_'i'
			intensity = Get value at time... 'curtime' Cubic
		
		
			intensity$ = fixed$ (intensity, 2)
			if intensity$ = "--undefined--"
				intensity$ = "0"
			endif

			values$=values$+f0$+"/"+intensity$+" "
		endfor
		appendFileLine: "results.txt",values$
	endfor