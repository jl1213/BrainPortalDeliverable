###### Brought to you by Joshua Lin (Duke '27, Neuroscience) and Sola Corrado (Duke '27, Neuroscience and Computer Science).

# Brain Portal: Anatomatch

## Description
This repository contains the complete work of the Social Format Touchscreen Display Group within the Brain Portal: Designing Multimedia Displays for Duke Neuroscience (2024–2025) Bass Connections team. Our subteam was tasked to develop an interactive neuroscience educational tool optimized for use on large touchscreen tables. Seeing the limitations of traditional, static neuroscience learning methods, we aimed to produce a dynamic, customizable experience that works for both students and instructors to foster exploration, curiosity, and intuitive learning.

Traditional educational games often lack the flexibility and scientific depth required for advanced learning environments. We saw the need to develop more effective tools for spatial reasoning and complex visualization - many of the current day tools often emphasize rote memorization over meaningful engagement, which might feel rigid, superficial, or unintuitive to students learning neuroanatomy. To address these limitations, we aimed to create a highly interactive, customizable platform that encourages users to explore real MRI data (sourced from the Duke Institute for Brain Sciences). Our goal is for students to gain geographic awareness in neuroscience. 

In our final product, users flip through coronal, sagittal, and axial slices of a real-life MRI brain, developing spatial architecture skills and intuition through an interactive experience designed to emulate a 3D perspective of the brain.

## **Notable Code Files**

### V4MCQ.cs
The V4MCQ.cs script manages the core logic of the touchscreen neuroscience quiz game. It initializes the scene (Start()), loads and parses quiz data from a CSV file (ReadCSVFile() and ParseCSVLine()), and displays questions and shuffled answer options (DisplayQuestion()). It handles player interactions by checking selected answers, updating scores, and providing immediate visual feedback (OnButtonTouch() and ResetButtonColors()). Correct or incorrect responses trigger transitions to a feedback scene (SwitchToSingleSlicer()). A countdown timer is managed in real time (Update()), adjusting a visible timer slider and enforcing penalties if time expires. 

### V2AnimationController.cs
The V2AnimationController.cs script controls interactive playback of brain MRI animations. It allows users to move forward or backward through animation frames at adjustable speeds using UI buttons. The script manages animation state (PlayAnimation()), dynamically calculates and displays the current frame number (UpdateFrameDisplay()), and supports smooth manual navigation (MoveForward() and MoveBackward()). It also ensures necessary components like colliders and coordinate display scripts are attached when animations load. Playback speed is adapted based on the number of frames to maintain consistent frame timing across different animations.

### SceneSwitcher.cs
The SceneSwitcher.cs script manages transitions between scenes in the neuroscience touchscreen application. It tracks global game state variables such as the current question index, the selected brain structure, and associated anatomical information. It loads feedback scenes (SwitchToFeedbackScene()) that display correct brain slices and structure information, updates visual elements (Start()), and resets game state when needed (ResetGameState()). It also controls movement back to the main quiz scene (LoadMainScene()), ensuring the player progresses through the educational flow smoothly.

### CollisionMaker.cs
The CollisionMaker.cs script generates interactive, clickable outlines based on coordinate data associated with brain images. It parses stored coordinates, draws solid line outlines, fills regions with semi-transparent meshes, and adds polygon colliders for tap detection. When users tap inside a correct outlined region, they are rewarded with points and progress through the educational flow. The script also handles penalty scoring for incorrect taps, making spatial learning and answer validation an intuitive part of the gameplay.

### ShowCoord.cs
The ShowCoord.cs script lets users place, display, and copy coordinate markers on brain images, generating a spatial answer key for educational feedback.

### TitleLoader.cs
The TitleLoader.cs script listens for user input (key press, mouse click, or tap) and loads the main application scene to start the experience.

### TimerBarController.cs
The TimerBarController.cs script manages the visual countdown timer during quiz interactions. It controls the timer’s progress bar (Slider UI), updates the timer each frame, and calculates a score multiplier based on how much time remains. This encourages quick, confident answers by rewarding players with higher scores for faster responses.


### InfoPanelController.cs
The InfoPanelController.cs script manages the information panel that displays anatomical, functional, and pathological details for selected brain structures. It automatically selects a default info tab based on the MRI viewing plane (coronal, axial, sagittal) and updates the panel when users tap corresponding buttons. Button highlights visually indicate the active category, helping users easily navigate between different types of neuroscience information.

### SettingsController.cs
The SettingsController.cs script manages the settings panel, offering scene restart, timer toggle, and application exit options for improved user control.


## Executing Program
This program must be uploaded to Unity using editor version 6000.0.35f1. The program was optimized for running on a Windows machine.

## Acknowledgements
This project is part of the larger Brain Portal initiative at the Duke Institute for Brain Sciences (DIBS) and was supported through the Bass Connections program at Duke University. We thank our mentors Dr. Len White and Professor Augustus Wendell for providing their valuable resources, expertise, and time.








