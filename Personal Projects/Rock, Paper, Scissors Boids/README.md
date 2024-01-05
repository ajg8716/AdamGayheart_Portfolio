# Project _Adam Gayheart_

[Markdown Cheatsheet](https://github.com/adam-p/markdown-here/wiki/Markdown-Here-Cheatsheet)

Name: Adam Gayheart

## Simulation Design

My simulator will be a rock paper scissors simulator where three types of npcs (rocks, papers, scissors) try to avoid and destroy other agents based on the rules of the game rock, paper, scissors. 

### Controls
   
Player has the ability to spawn new agents in the scene. to spawn new agents, player must hover over an area they would like to spawn a new agent with their mouse. Then, by clicking the keys(a, s, d) the player will spawn a new agent into the scene.
      A: spawns a rock
      S: spawns a pair of scissors
      D: spawns a paper

## _Scissors_

will be represented by a scissor sprite. will seek paper and flee from rock. 

### _Scissor chase Paper_

**Objective:** seek paper elements to collide with paper. 
when colliding with paper: turns paper into scissors element 

#### Steering Behaviors
- Obstacles – 
Hand of God Element
- Seperation – 
other Scissor Agents

Seeks paper when in range of paper
Flees rock when in range of rock
wanders and flocks with others of same type in rnage when not in range of either rock or paper
   
#### State Transistions

- _List all the ways this agent can transition to this state_
   - _eg, When this agent gets within range of Agent2_
   - _eg, When this agent has reached target of State2_

   within range of paper, will seek paper
   within range of rock, will flee rock
   when colliding with rock, turns into rock
   else: wanders
   
### _Scissor Flee Rock_

**Objective:** 
scissor flees rock

#### Steering Behaviors

- _List all behaviors used by this state_
- Obstacles - _List all obstacle types this state avoids_
Hand of God obstacles
- Seperation - _List all agents this state seperates from_
seperates from other scissors in the flock
   
#### State Transistions
- _List all the ways this agent can transition to this state_

in range of rock, flees rock


## _Rock_

Rock element, represented by rock sprite

### _Rock chase Scissors_

**Objective:** _A brief explanation of this state's objective._

rock seeks to collide with scissors

#### Steering Behaviors

- Obstacles -
Hand of God Element
- Seperation -
Seperates from other rocks in the flock


when in range, rock will seek the scissors
   
#### State Transitions

- _List all the ways this agent can transition to this state_
when in range of scissors.
when colliding with scissors, scissors become rock element
   
### _Rock Flee Paper_

**Objective:** _A brief explanation of this state's objective._
Rock flees from paper elements

#### Steering Behaviors

- Obstacles -
Hand of God Element
- Seperation -
Seperates from other rocks in the flock

rock flees from paper position
   

#### State Transistions

when in range of paper

## _Paper_
Paper element, represented by paper sprite

### _Paper chase Rock_

**Objective:** _A brief explanation of this state's objective._

paper seeks to collide with rock

#### Steering Behaviors

- Obstacles -
Hand of God Element
- Seperation -
Seperates from other pepers in the flock

when in range, paper will seek the rock
   
#### State Transistions

when in range of of rock.
   
### _Paper Flee Scissors_

**Objective:** 
Paper flees from scissors elements

#### Steering Behaviors

- Obstacles -
Hand of God Element
- Seperation -
Seperates from other papers in the flock

paper flees from scissor position
   
#### State Transistions

when in range of scissor

## Sources

I will make all of my sprites

## Make it My Own

I am adding a third Agent, Paper to get full rock paper scissors gameplay. 
Player will be able to spawn in new agents. They will choose which one they want to spawn and be able to click on the screen, and spawn that agent into the scene

## Known Issues

agents will be sent flying off screen and the come shooting back and I am confused as to why it is happening/ it only happens when agents are fleeing or seeking

