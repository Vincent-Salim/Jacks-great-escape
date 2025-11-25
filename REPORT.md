# Project 2 Report

Read the [project 2
specification](https://github.com/feit-comp30019/project-2-specification) for
details on what needs to be covered here. You may modify this template as you
see fit, but please keep the same general structure and headings.

Remember that you should maintain the Game Design Document (GDD) in the
`README.md` file (as discussed in the specification). We've provided a
placeholder for it [here](README.md).

## Table of Contents

- [Evaluation Plan](#evaluation-plan)
- [Evaluation Report](#evaluation-report)
- [Shaders and Special Effects](#shaders-and-special-effects)
- [Summary of Contributions](#summary-of-contributions)
- [References and External Resources](#references-and-external-resources)

## Evaluation Plan

Our goal: determine if our game not only works without bugs but also is engaging for our chosen age group. We want to make sure the storyline and difficulty resonates with them and so our evaluation is built around that.

Target Audience: Older Kids & Teens

Plan for recruitment: some of our team members have younger siblings so we would recruit them and their friends. The only qualifying criteria is that they fit in the age range which will be easy to identify (asking/confirming their age/school grade). Should easily be able to arrange 10 participants with our connections.

We need to consider time constraints and access to people of this demographic when selecting our evaluation techniques. Ideally, we would choose techniques that do not necessarily require us to be in the room with the participants/time consuming methods.

### Evaluation Technique 1: Think Aloud.

How this would work: get the participant to vocalise what they are thinking while they play the game and screen record at the same time. (Make sure they vocalise when they hit play).

Given that we do not have child brains, it would be very helpful to see how they operate when playing our game.

And because this game targets a younger demographic – we want to avoid using a technique with too much developer involvement as kids and teens should be able to play this game unguided.

We will be collecting audio and video data – this method would be time consuming to scale (but in the situation of 5 participants, would be reasonable to collect data).

How we would analyse the data: after listening through, we would make notes of any holes/traps participants fall into, noting any sections of frustration. Also, we would note whether participants have any trouble getting into it/understanding the game. This would indicate what needs to be made clearer. We would look at our findings across participants to see what to prioritise in development.

### Evaluation Technique 2: Questionnaire

How this would work: After participants have had a go at playing the game, they would fill out a pre-made questionnaire. We will include scalar questions + 1 long-answer general question as an opportunity for participants to give us any feeback they felt they haven’t covered.

The scalar questions will target a range of topics including mental workload, usability, engagement etc.

This will be very simple to implement and will help us to understand how the game is viewed by kids/teens in retrospect (as they reflect). It will also help us to get a general understanding of how it is perceived as scores are easily compared.

We would use a service like google forms and receive numerical and text data to analyse. The numerical data can be averaged to get a better understanding of the response to the game and then the text will be analysed and compared to see if any feedback is repeated.

A big thing here is – if the game is consistently being ranked as easy, we will increase difficulty, or consistently boring, we will add more visual interest etc.

### Timeline:

We are taking a little longer with development due to the art we need to add. Ideally, most of the art would be implemented before evaluating as this is a kid’s game and the visual design is a key aspect.

13/10 - begin recruitment of participants – Sarah (on correspondence)

20/10-23/10 - evaluation period – Sarah to manage

24/10 - analyse results – Sarah to report on findings

## Evaluation Report

Our evaluation went well and we were able to source participants – albeit it took longer than anticipated. Our participants had the average age of 13.6 years old. So we managed to meet our key demographic.

Key takeaways after listening to the audio from the think aloud evaluation:

- It was clear from the audios that most of the kids and teens had trouble determining the controls in the game. Some testing and playing with the keys is normal - but we want to make sure our gameplay is clear.
- Another thing that was mentioned repeatedly was the fact that we lacked some graphics – particularly on the transition start and end scenes.
- Level 2 was very easy and unexciting.
- Level 3 was quite difficult.
- Some weren’t explicitly clear on the aim of the game.

Key takeaways from the questionaire evaluation results:

- There was an overall positive response. Most participants enjoyed the game – particularly the art and graphics. Average overall rating was 7.6/10.
- The ratings for the levels were: Level 1: 8.0/10, Level 2: 6.8/10, Level 3: 7.2/10.
- They rated the levels on difficulty: Level 1: moderate (3.4/5), Level 2: easy (1.2/5), Level 3: difficult (4.6/5).
- They also mentioned a lack of transition art, no pause and some confusion over controls. Also mentioned the lack of sound.
- A lot of the feedback was consistent with Think Aloud evaluation.

Our changes in response:

1. Finished implemented all our start scene + transition + end scene graphics and implemented a pause feature. So essentially polished off the game edges.
2. To address level 2 easyness – added in more dishes falling at a high rate + implemented bubble health system so that players can die and have to restart the level.
3. To address level 3 difficulty – adjusted the knife positions to make it easier for player to complete – while still maintaining it to be somewhat difficult and it is our hardest level.
4. Added instructions with key controls to make it explicit to the user what keys to press. They can refer to this in the situation of confusion.
5. Added a arrow to all levels that the play needs to follow to get the treasure – also added instruction to get to the treasure in the beginning.

## Shaders and Special Effects

Our project implements two custom vertex/fragment shaders written in HLSL, as well as a particle system for environmental effects. Both shaders are built to enhance the visual effects of our game. Each shader file is clearly commented to explain the purpose and function of key code sections, including how they relate to different stages of the rendering pipeline. Both shaders operate entirely on the GPU as fragment-based effects, reducing the need for CPU-side post-processing

| Shader                                                                                                                              | Material                                                                                                                       | Associated Textures                                                                                           |
| ----------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------ | ------------------------------------------------------------------------------------------------------------- |
| [Filmify.shader](https://github.com/feit-comp30019/2025s2-project-2-pixelate/blob/main/Assets/Shaders/Filmify.shader)               | [Filmify.mat](https://github.com/feit-comp30019/2025s2-project-2-pixelate/blob/main/Assets/Material/Filmify.mat)               | —                                                                                                             |
| [HeatDistortion.shader](https://github.com/feit-comp30019/2025s2-project-2-pixelate/blob/main/Assets/Shaders/HeatDistortion.shader) | [HeatDistortion.mat](https://github.com/feit-comp30019/2025s2-project-2-pixelate/blob/main/Assets/Material/HeatDistortion.mat) | [noise.png](https://github.com/feit-comp30019/2025s2-project-2-pixelate/blob/main/Assets/Art/Noise/noise.png) |

<p align="center">
  <table>
    <tr>
      <td align="center" width="33%">
        <img width="100%" alt="Heat Shader"
             src="https://github.com/feit-comp30019/2025s2-project-2-pixelate/blob/main/Media/heatdistortion.gif"><br>
        <em>Heat Distortion Shader</em>
      </td>
      <td align="center" width="33%">
        <img width="100%" alt="Filmify Shader"
             src="https://github.com/feit-comp30019/2025s2-project-2-pixelate/blob/main/Media/filmify.gif"><br>
        <em>Filmify Shader</em>
      </td>
      <td align="center" width="33%">
        <img width="100%" alt="Smoke Particles"
             src="https://github.com/feit-comp30019/2025s2-project-2-pixelate/blob/main/Media/smoke.gif"><br>
        <em>Smoke Particles</em>
      </td>
    </tr>
  </table>
</p>

### Filmify Shader

Purpose:
Simulates an old-film visual aesthetic by increasing the grain and some scanline effects.
Implementation:
The vertex stage passes screen UVs to the fragment shader, where the built-in Unity variable \_Time.y is used to generate a noise UV through a perlin noise function. Parameters such as \_Strength and \_Aspect are exposed as variables to allow easy adjusting within the material inspector.

In the rendering pipeline, the shader runs as a full-screen post-processing effect using a custom material attached to a RenderTexture. This approach ensures the shader executes after geometry rendering and UI/Canvas elements.

### Heat Distortion Shader

Purpose:
Creates refractive distortion to simulate heat haze around hot surfaces.
Implementation:
This shader is also applied to a RenderTexture via a material, without any C# scripting. The effect is achieved entirely within the vertex and fragment stages. The fragment shader samples a distortion (normal) texture and offsets the UV coordinates, according to \_Time.y, before sampling the scene color texture. These offsets are scaled by the \_DistortionStrength property, which is exposed in the material inspector for manual adjustment.

Because it operates on a render texture, the shader distorts the entire rendered frame rather than a single mesh. This placement in the rendering pipeline makes it ideal for post-processing effects such as heat shimmer.

### Particle System

Purpose:
Creates a visual warning system for fire hazards by emitting smoke particles before fire erupts, enhancing gameplay readability and adding atmospheric depth to the kitchen environment.
Implementation:
The smoke particle system is integrated with the FireTrap hazard objects throughout Level 1. Each FireTrap GameObject has a child ParticleSystem component configured to simulate rising smoke. The system uses Unity's particle system with custom parameters.
The particle system is controlled via coroutine timing in the FireTrap script, starting 0.5 seconds before fire activation (warning phase) and lingering 0.3 seconds after fire deactivation. This creates a predictable visual cue that helps players anticipate and avoid the hazard.

## Summary of Contributions

| Vincent                                                                                                                      | Sarah                                                                                           | Amy                                                                                                                         | Lionel                                                                                                                                                             |
| ---------------------------------------------------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------------------- | --------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| **TEAM LEADER**                                                                                                              |                                                                                                 |                                                                                                                             |                                                                                                                                                                    |
| Acted as our leader.<br>Level 3 implementation.<br>Both shader implementations.<br>User input + basic run/jump/object interaction.<br>Had to fix many issues that came up with other team members' implementations| Level 2 implementation.<br>Player animation implementation.<br>Evaluation organiser + analysis. | Level 1 implementation.<br>Artist – background + some objects.<br>Implemented start + transition scenes.<br>Added parallax. | Artist leader – designed the style of the game (audio, graphics, and helped implement).<br>Implemented particle system.<br>Implemented arrow pointing to treasure. |

## References and External Resources

### References  
Tutorials, guides, and technical discussions.

#### Sprite  
https://learn.unity.com/tutorial/introduction-to-sprite-animations  

#### Sprite Sheet Splitting  
https://discussions.unity.com/t/split-a-sprite-image-into-multiple-animations/710477  

#### Sprite Animation Video Guide  
https://www.youtube.com/watch?v=TbGEKpdsmCI  

#### 2D Physics and Object Stacking  
https://discussions.unity.com/t/pile-gameobjects-keep-sliding-when-stacked-in-2d/873387  

#### Object Shaking Fix  
https://discussions.unity.com/t/objects-are-shaking-when-stacked/722764  

#### Filmic Filters  
https://danielilett.com/2019-11-20-tut3-5-filmic-filters/  

#### Heat Distortion Shader  
https://lindenreidblog.com/2018/03/05/heat-distortion-shader-tutorial/  

#### Particle Effects  
https://learn.unity.com/course/2d-beginner-adventure-game/unit/enhance-and-polish/tutorial/create-2d-particle-effects?version=2022.3  

#### Audio Implementation  
https://www.youtube.com/watch?v=gd_DJsfiaKQ  

#### Parallax Scrolling  
https://medium.com/@Code_With_K/parallax-background-in-unity-fd8766d5a9bd  

#### Parallax Video Guide  
https://www.youtube.com/watch?v=zit45k6CUMk  

#### Backgrounds  
https://www.google.com/imgres?q=old%20timey%20kitchen%20background&imgurl=https%3A%2F%2Ft4.ftcdn.net%2Fjpg%2F06%2F61%2F20%2F35%2F360_F_661203556_kiYhIMGajQtemIDBFGnGwJM5jjZbC3ux.jpg  

#### Style References - Sprites  
https://www.spriters-resource.com/pc_computer/cupheaddontdealwiththedevil/  

### Resources  
Assets, inspiration, and tools.

#### Sound Effects  
https://pixabay.com/sound-effects/  

#### AI Music Generation  

https://suno.com/create?wid=default  
