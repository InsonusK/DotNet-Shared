---
name: Validators writing skill
version: 1.0.0
description: This skill provides guidelines and best practices for writing validators in the Command Validation library. It covers when to use this skill, required input data, prerequisites, and step-by-step instructions on how to implement effective validators for command validation scenarios.
---

# When to use this skill
Use this skill when you need to create custom validators for commands in the Command Validation library. This skill is essential for ensuring that your commands are validated correctly before execution, helping to maintain data integrity and enforce business rules.

# Input data:
You need define these information before start:
- Object to validate: The specific command or data structure that requires validation.
- Validation rules: 
  - The specific rules and conditions that the object must meet to be considered valid.
  - Error messages: Custom error messages that should be returned when validation fails, providing clear feedback on what went wrong.
  - Error codes: Optional error codes that can be used to categorize validation errors for easier handling in the application.
  - Error severity: Optional severity levels for validation errors (e.g., warning, error, critical) to help prioritize issues.
  
# Requirements
You can use thise skill if:
- You are working on a project that utilizes the need to validate commands or data structures before processing them.

# How to use it
1. Define project where validated commands or data structures are located.
2. Define or create folder `Validators` in located project.
3. Define or create folder `Validators/Properties` in located project to store validators for specific properties of commands or data structures.
4. If project contain Models, Entites, define or create folder `Validators/Models`, `Validators/Entities`in located project to store validators for these objects.
5. Create a new class for your validator, inheriting from `AbstractValidator<T>` where `T` is the type of object you want to validate.
6. Define `PropertyValidator` for each property of the object that requires validation. This involves creating a new class that inherits from `AbstractValidator<TProperty>` and implementing the validation rules for that specific property. Every error in `PropertyValidator` should have a unique error code to allow for precise error handling and debugging.
7. Use created `PropertyValidator` in the main validator class to validate

