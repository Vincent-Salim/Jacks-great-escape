**Game Design Document: Jack’s Great Escape**  
<span class="mark">The document should be well-written, easy to read,
and formatted appropriately in "GitHub flavoured" markdown. The
README.md file in the given template repository must be utilised to
capture the design document.</span>

# Game Overview

The game is based on a retelling of the well-known children’s fable,
Jack and the Beanstalk. It is set in the kitchen of the Giant, and Jack
is trying to escape whilst stealing some valuable possessions. The
player is Jack, who is stuck in a ginormous kitchen tabletop, having to
get past the sink, the stovetop and the cutting board, whilst picking up
treasures including a bag of gold, a gold-laying hen, and a magic harp.
The objective of the game is to dodge all obstacles and pick up the
items to make it to the end, where the beanstalk is located so that Jack
can head back down to safety

Genre

The primary genre which this game falls under is Action, it is a running
game which employs elements from classic platformers such as Super Mario
Bros, such as dodging obstacles and timing jumps. This is demonstrated
in when the player, Jack, must dodge dangers on the stovetop, sink, and
chopping knives.

Sub-genres of this game include the Adventure genre, as the multi-staged
design of this game follows a clear storyline with narrative, and goal
of collecting valuables and preserving safety. It also falls under the
Collectathon genre as it involves collecting specific valuables to
succeed in the game. As the overall storyline of the game is inspired by
a common children’s story, the game also is part of the Fairy Tale
genre.

Target Audience

This game is targeted for children and teenagers, as it follows the
story from a classic fable. It should be familiar to children and
teenagers, bringing about a sense of wonder as they get see a childhood
story brought to life, and perhaps nostalgia for teenagers about the
story. However, this game is not limited to children and teenagers and
can suit players of any age.

Unique Selling Point

However, unique to mainstream platform games, this game plays with the
perspective as you experience a kitchen setting from a tiny person’s
perspective. It is also rare that modern games follow a classic fable
storyline, as this game does, providing familiarity and keeping players
invested as they already know the backstory of the characters. The art
style chosen to render this game in, discussed in detail in the
following paragraphs, is also unique and aims to instil a sense of
nostalgia for the vintage 1930’s rubber hose animations, reminiscent of
iconic characters like Oswald the Lucky Rabbit, Mickey Mouse, Betty
Boop, and Popeye, taken from the early era of animation.

# Story and narrative 

The game is based on a retelling of the well-known children’s fable,
Jack and the Beanstalk.

Backstory

The main player is Jack, who sold his cow for magic beans that grew into
a towering beanstalk. Climbing it, the Jack finds himself in a giant’s
house in the clouds, unsure of who lives there but driven by a desire to
steal the giant’s valuable possessions from the kitchen tabletop due to
his own unsatisfactory financial situation. Jack is tiny compared to
everything in the kitchen, navigating oversized knives, objects in the
sink, and stovetops.

The backstory of how Jack came to be there is not shown in the game, as
it is taken from a classic fable so is assumed knowledge, and it is also
not necessary knowledge to play the game in the case the player has not
heard of the story of Jack and the Beanstalk.

Narrative

The player begins the game in the giant’s kitchen, on the tabletop,
ready to navigate their way through the obstacles to the treasures, and
finally back to the safety of the beanstalk.

At first, the giant is unaware of Jack’s presence, but as Jack collects
treasured items like a bag of gold, a gold-laying hen, and a magic harp,
the giant becomes increasingly angry and starts trying to eliminate Jack
by turning on stoves and swinging knives. The objective is for the Jack
to dodge all hazards, gather the valuable items, and make it across the
kitchen bench to reach the beanstalk, where Jack can escape back down to
safety.

Jack

Jack is the main character of the game. He is poor and needs money to
care for himself and his mother, and his worn clothes reflect his humble
life. Jack is also reckless and cheeky, showing no hesitation in
stealing from the giant to get what he wants.

<img src="/media/image3.png" style="width:3.94038in;height:2.78646in" />

Jack front face

<img src="/media/image4.png" style="width:4.78584in;height:3.49036in" />

Jack side view and running

Giant

The giant serves as the antagonist, with a short temper and a strong
dislike for having his possessions taken - understandably so. He is
clumsy and loud, often bellowing his famous “Fee Fi Fo Fum.” The giant
lives alone in a simple house high in the clouds, cooking his own meals
every day, never receiving visitors, and never having encountered Jack
before. His clothing is simple, matching his straightforward, solitary
lifestyle.

<img src="/media/image5.png" style="width:3.81073in;height:2.9836in" />

The giant front face (this is what we will see + arm movement)

# Gameplay and mechanics 

Player Perspective

In the game, you are viewing from a third person perspective. That is to
say that Jack will be on screen and you will viewing and controlling him
from side on. The camera moves with Jack so the view will always be
centred on Jack and he is visible in the frame.

<img src="/media/image6.png" style="width:4.13238in;height:2.24783in" />

Jack can interact with objects in the game world – they can hit him, he
will spring back.

Controls

Jack can interact with objects in the game world – they can hit him, he
will spring back, he can jump on top of objects and can collect
treasures.

| **Key Input** | **Action**                                 |
|---------------|--------------------------------------------|
| W             | Jump up                                    |
| A             | Move left (backward in Jack’s perspective) |
| D             | Move right (forward in Jack’s perspective) |
| E             | Pick up a treasure                         |
| Space         | Power up                                   |

Progression

The goal of the game is that the player will progress through 3 levels
collecting treasure and not dying. The player will want to get past all
three level to collect the treasures and escape the kitchen.

<table>
<colgroup>
<col style="width: 31%" />
<col style="width: 33%" />
<col style="width: 35%" />
</colgroup>
<thead>
<tr>
<th><p><strong>Level 1:</strong></p>
<p><u>Antique Oven</u></p>
<p>Easiest level. Giant unaware of Jack’s presence, Jack simply has to
manoeuvre through without being burnt.</p></th>
<th><p><strong>Level 2:</strong></p>
<p><u>Kitchen Sink</u></p>
<p>Difficulty increases. Giant is now aware of Jack and being more
aggressive in trying to eliminate Jack.</p></th>
<th><p><strong>Level 3:</strong></p>
<p><u>Chopping Board</u></p>
<p>Difficulty increases again. Giant is now very angry and</p>
<p>trying to chop Jack.</p></th>
</tr>
</thead>
<tbody>
<tr>
<td>Jack loses a life by being burned (running into/getting blasted by
the fire).</td>
<td><p>Jack dies by drowning (runs out of bubbles).</p>
<p>Similar to Minecraft bubbles.</p></td>
<td>Jack loses a life by being chopped (hit with a knife).</td>
</tr>
<tr>
<td>Jack has 3 lives.</td>
<td>Jack has 5 bubbles that he begins to lose when he is under the water
level. Thy regenerate when he gets above the water level.</td>
<td>Jack has 3 lives (this is separate to the previous level’s
life).</td>
</tr>
<tr>
<td>Jack wins level 2 by not dying and collecting gold-laying hen.</td>
<td>Jack wins level 1 by not dying and collecting bag of gold.</td>
<td>Jack wins the game by not dying and collecting golden harp.</td>
</tr>
</tbody>
</table>

See the next section for further info on how each level works.

<span class="mark">Gameplay Mechanics: What gameplay mechanics are used
in the game? What actions can the player take? What rules or "laws"
govern the game world? What are the core mechanics that make the game
fun? How do these mechanics fit in with the overarching theme?</span>

- <span class="mark">Player can run and jump</span>

- <span class="mark">Player lives off 3 hearts, which can be lost when
  the player hits an obstacle, loss of all hearts results in the
  player’s death and having to restart the level</span>

- <span class="mark">This fits into the overarching theme as</span>

Gameplay Mechanics

# Levels and world design 

<img src="/media/image6.png" style="width:5.05849in;height:2.7516in" />

Game World

We will have a 2.5D setting with sideview scrolling for our game. The
camera follows our character leaving him near the middle of the screen
whilst he traverses the kitchen environment. There will be at least 3
levels and for all levels the primary movement will be left and right
movement with the addition of going up by jumping/swimming. There will
be no mini map or map as each obstacle will be displayed on the screen.

Objects

- **Objects:** What objects are in the game world? What do they look
  like? What are their roles? How do they interact with the player? How
  do they interact with each other?

<span class="mark">The main objects of the game world will be the
kitchen countertop (oven, sink + chopping board). This will be the
platform our character traverse on. On the countertop will be several
obstacles all of which are kitchen appliances or items. These items will
either prevent the player from continuing onwards by blocking the path
or damage the player. These include: knives, faucets, fire, logs and
dishes. Other than the kitchenware there will be an npc, the giant, that
will also be form an obstacle. The giants hand will act as an obstacle
and stop the player, the hands may will be able to interact with the
kitchen appliances. Each obstacle will have a mechanic that the player
will need to use to traverse across the countertop. The mechanics are
listed below.</span>

Physics

The physics of the game will include gravity, object collision,
instantaneous left right movement and, variable jump height (dependent
on keypress duration). The objects will move based off the player or the
giant interacting with it. Each object will interact independently of
each other objects as they do not move on their own.

<u>Levels</u>

Level 1

Jack tries to cross the oven whilst avoiding the flames that are
bursting from the logs. He must hop on the logs that are no longer
burning either due to being completely burnt or has not begun burning
yet. Getting hit by the fire will reduce his heart by
1.<img src="/media/image.jpg" style="width:6.01042in;height:1.52083in" />

Level 2

Jack jumps in the sink and the giant turns on the faucet beginning the
level. Jack now must avoid the falling dishes and kitchenware the giant
is placing and use them to escape the sink. Jack will jump/swim
(depending on where the water level is) from the kitchenware placed to
reach a higher elevation to eventually escape the
sink.<img src="/media/image2.jpg" style="width:6.01042in;height:4.125in" />

Level 3

Jack tries to cross the chopping board whilst the giant and his knives
try to stop jack by chopping him up. The knives all move at different
frequencies (knifes later in the level move
faster).<img src="/media/image3.jpg" style="width:6.01042in;height:2.57292in" />

# Art and audio 

Art Style: 1930s rubber hose animation. Simple rounded forms, flexible
limbs that move like “rubber hoses”. Fluid, bouncy movement for
characters. A game with a similar art style is really just Cuphead.

<img src="/media/image.png" title="Inserting image..."
style="width:3.09375in;height:1.74024in" /><img src="/media/image2.png" title="Inserting image..."
style="width:3.02306in;height:1.70047in" />

**Character Design Specifics:**

- <u>Jack</u>: pie-cut eyes, simple oval head, stick-thin limbs that can
  stretch when jumping between giant utensils

- <u>The Giant</u>: exaggerated movements, since we only ever see his
  body – we will his massive hands reaching across the screen with a
  characteristic bouncy movement, as well as exaggerated swaying and
  walking in the background (angry).

- <img src="/media/image7.png" style="width:4.38288in;height:2.30448in" />

**Environmental Art Style:**

- Everything should feel oversized and imposing from Jack's tiny
  perspective

- Kitchen surfaces could have that classic 1930s cartoon texture -
  simple, high-contrast shading

- Steam from pots, water droplets, and fire effects should move with
  that fluid, elastic quality

<img src="/media/image8.png" style="width:3.38688in;height:2.54467in" /><img src="/media/image9.png" title="Inserting image..."
style="width:2.73223in;height:2.55948in" />

Sound and Music: To complement the 1930s style, we plan on using jazzy
upbeat music for the backing track, with each area having a different
composition to reflect the area’s mood. For example, sneaky ragtime
piano for stealth sections, frantic big band when the Giant notices you,
triumphant swing when collecting treasures. We can match some of the
parkour elements to the music, making platforms appear and disappear in
sync. Jack's footsteps could also be synced to musical beats, and the
Giant's movements create bass drum hits. Environmental sounds would also
be in a jazzy style, with bubbling pots as percussion, knife chopping as
rhythmic elements.

# User interface

Describe the game's UI, including things like health bars, score
displays, and menu screens. Detail both the functionality and aesthetic
design of the UI. Include any diagrams, wireframes or sketches to assist
with visualisation. Ensure that the UI design is consistent with the
game's overall aesthetic.

<img src="/media/imagea.png" style="width:4.2894in;height:3.5001in" />

Design breakdown

**Health System Design:**

- Instead of generic hearts, we use rubberhose-style hearts that bounce
  and deflate when lost.

- When near drowning, animated bubble effects will pulse to the jazz
  beat. The bubble effects grow larger and more frantic as you get
  closer to drowning, accompanied by muted jazz instruments getting
  "underwater" distorted.

- Damage (burning,chopping) will make Jack's limbs stretch and snap back
  in classic rubberhose style.

**Treasure Collection:**

- Show the collected items (gold bag, hen, harp) in ornate 1930s-style
  frames at screen corners

- Items could have subtle animations to make the game feel move alive –
  the bag of gold could jiggle, the hen could blink, the harp strings
  could shimmer and strum.

<img src="/media/imageb.png" style="width:5.65077in;height:2.0591in" />

**Interactive UI Elements:**

- "Press E" (pick up with object) prompts styled as cartoon speech
  bubbles

- Level transition screens designed like old cartoon title cards

- Movement indicators can be shown with subtle arrow overlays when
  approaching climbable objects, styled as vintage cartoon motion lines.

<img src="/media/imagec.png" style="width:3.01045in;height:2.29393in" /><img src="/media/imaged.png" style="width:2.31998in;height:2.29115in" />

# Technology and tools

The game mechanics will be made in Unity. Version control and code
management will be done through GitHub. The 2D objects will be drawn in
Procreate or Photoshop as our team members have experience with these
drawing softwares. The audio editing will be done in audacity as our
team member have experience in it, and it fits our simple audio editing
requirements. The versions of each software will be included once we
begin.

# Team communications, timelines and task management

Deadlines:

| Milestone 1 | Team Declaration         | 1/9   |
|-------------|--------------------------|-------|
| Milestone 2 | Game Design Document     | 15/9  |
| Milestone 3 | Team Member Evaluation 1 | 25/9  |
| Milestone 4 | Evaluation Demo          | 13/10 |
| Milestone 5 | Gameplay Video           | 20/10 |
| Milestone 6 | Final Submission         | 5/11  |
| Milestone 7 | Team Member Evaluation 2 | 7/11  |
| Milestone 8 | Team Feedback Reflection | 9/11  |

\*All submissions due at 5pm

<u>Plan for communication</u>

- Meetings twice a week:

  - Monday 1pm (in person, working meeting)

  - Friday 7pm (online stand-up, subject to time change)

<!-- -->

- All communication outside of meetings must occur on discord in the
  “Team Pixelate” chat – for long, ongoing discussions, use a
  thread/separate chat.

- A document listing the breakdown of work and its progress will be
  updated to ensure the team stay on track. It will include meeting
  minutes.

<u>Plan for work distribution</u>

- Work should be distributed in a fair manner, considering the strengths
  and interests of all team members.

- All work assigned will be documented in the progress tracker doc.

# <span class="mark">Possible challenges</span>

Discuss potential difficulties you foresee during the development of
your game, and how your team plans to address these issues as they
arise. These could be technical challenges, time constraints, or
anything else that might impact your team's ability to complete the
project. Prototyping and testing can be particularly helpful here, and
you should trial these out in the next milestone.

<u>Technical Challenges</u>

- Merge conflicts

- Rendering

- Bugs in the game

- Integrating the art style into unity

<u>Time Constraints</u>

- Team members not making their deadlines

- Things taking longer than expected

<u>Other</u>
