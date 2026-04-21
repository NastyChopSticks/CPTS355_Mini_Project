# PostScript Interpreter (C#)

## Overview
This project is an implementation of a subset of the PostScript language in C#. It supports 47 core PostScript commands, including stack manipulation, arithmetic operations, dictionary handling, control flow, and input/output.

The interpreter was developed over the course of approximately two weeks as a learning project. While it demonstrates key concepts such as stack-based execution and scoping, it is not a fully robust implementation. Some commands may have edge-case issues, and the scoping system (static and dynamic) is implemented in a simplified form.

This project is intended primarily as a **learning tool** for understanding programming language design and interpreter construction.

---

## Features
- Stack-based execution model
- 47 implemented PostScript commands
- Arithmetic and logical operations
- Dictionary support
- String manipulation
- Flow control (if, ifelse, for, repeat)
- Input/output operations
- Basic static and dynamic scoping

---

## Implemented Commands

### Stack Manipulation
| Command | Description |
|--------|------------|
| `exch` | Swap top two elements |
| `pop` | Remove top element |
| `copy` | Duplicate top *n* elements |
| `dup` | Duplicate top element |
| `clear` | Clear entire stack |
| `count` | Count elements on stack |

---

### Arithmetic
| Command | Description |
|--------|------------|
| `add` | Addition |
| `sub` | Subtraction |
| `mul` | Multiplication |
| `div` | Division |
| `idiv` | Integer division |
| `mod` | Modulo |
| `abs` | Absolute value |
| `neg` | Negation |
| `ceiling` | Round up |
| `floor` | Round down |
| `round` | Round to nearest integer |
| `sqrt` | Square root |

---

### Dictionary
| Command | Description |
|--------|------------|
| `dict` | Create dictionary with capacity |
| `length` | Number of key-value pairs |
| `maxlength` | Dictionary capacity |
| `begin` | Push dictionary onto stack |
| `end` | Pop dictionary stack |
| `def` | Define key-value pair |

---

### Strings
| Command | Description |
|--------|------------|
| `length` | String length |
| `get` | Get character at index |
| `getinterval` | Extract substring |
| `putinterval` | Replace substring |

---

### Boolean & Bitwise
| Command | Description |
|--------|------------|
| `eq`, `ne` | Equality / inequality |
| `ge`, `gt`, `le`, `lt` | Comparisons |
| `and`, `or`, `not` | Logical/bitwise ops |
| `true`, `false` | Boolean constants |

---

### Flow Control
| Command | Description |
|--------|------------|
| `if` | Conditional execution |
| `ifelse` | Conditional branching |
| `for` | Loop with step |
| `repeat` | Repeat n times |
| `quit` | Terminate interpreter |

---

### Input / Output
| Command | Description |
|--------|------------|
| `print` | Print string |
| `=` | Print value |
| `==` | Print PostScript-style value |

---

## Limitations
- Developed in a short timeframe (~2 weeks)
- Some commands may contain bugs or incomplete behavior
- Static and dynamic scoping are implemented but not fully robust
- Not a complete PostScript implementation

---

## Purpose
This project was built to explore:
- Interpreter design
- Stack-based languages
- Static vs. dynamic scoping
- Object-oriented design in C#

---

## Getting Started

### 🪟 Windows

**Option 1: Using Visual Studio**
1. Clone the repository  
2. Navigate to: CPTS355_Mini_Project/PostScript_Interpreter
3. Open `PostScript_Interpreter.sln` in Visual Studio  
4. Build and run the project  

**Option 2: Running the compiled executable**
1. Clone the repository  
2. Navigate to: CPTS355_Mini_Project/PostScript_Interpreter/bin/Debug/net8.0
3. Run the `.exe` file  

---

### 🍎 macOS

**Install dependencies**
```bash
brew install --cask dotnet-sdk@8
cd CPTS355_Mini_Project/PostScript_Interpreter
dotnet run
---

## Notes
This is a learning-focused implementation and should not be considered production-ready.
