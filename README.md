AvoidAgent Unity ML-Agents Project

Project Overview

This project demonstrates a reinforcement learning (RL) agent using Unity ML-Agents. The agent is trained to navigate an environment while avoiding obstacles. The goal is to develop an agent that can survive and move safely using sensor observations.

Features
	•	Agent Controller: A Rigidbody-based agent controlled via ML-Agents.
	•	Ray Perception: The agent uses multiple raycasts to detect obstacles around it.
	•	Movement: Continuous actions control movement (X, Z) and jumping.
	•	Reward System:
	•	Small survival reward for each step.
	•	Penalty when colliding with obstacles.
	•	Randomized Start: The agent spawns in a safe random location at the beginning of each episode.

Training
	•	Algorithm: PPO (Proximal Policy Optimization)
	•	Max Steps: 500,000 (can be adjusted in config/avoid_env.yaml)
	•	Observation Space:
	•	3 values for agent velocity
	•	2 values for agent position
	•	8 values from raycasts
	•	Action Space: 3 continuous actions (moveX, moveZ, jump)

Files
	•	config/avoid_env.yaml: Training configuration.
	•	results/run1/AvoidAgent/: Contains trained models, ONNX model, and training logs.
	•	Assets/Scripts/RayAgentController.cs: Agent behavior script.

How to Run
	1.	Open the Unity project.
	2.	Drag the AvoidAgent.onnx model to the Agent’s Behavior Parameters in the Unity Editor.
	3.	Press Play to see the agent controlled by the trained model.
	4.	(Optional) Run TensorBoard to visualize training metrics:tensorboard --logdir=results

  Notes
	•	Ensure the agent’s Behavior Type is set to Inference Only when using the trained ONNX model.
	•	Training requires Python and ML-Agents environment to be running.
	•	Adjust hyperparameters and max_steps in the YAML file for faster or longer training.
