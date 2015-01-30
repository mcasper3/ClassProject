# ClassProject
Assembler
-------------------------------------------------------------------------------------------------
Operation of Assembler:
-------------------------------------------------------------------------------------------------
This assembler allows for the conversion of an assembly file (whose syntax is specified below) into a .mif file. To use the
the assembler, run the Assembler.exe and read the menu options. Select one of the four options. The first option displays the
example file with correct syntax which can also be found at the end of this file. The second option displays a helpful hint for
what your next choice should be. The third option converts a provided name of an assembly file into a .mif file.

=========
Instruction syntax:
=========
R-Type:
---------
add/sub/and/or/xor $ra $rb $rc<br>
// performs an operation of add/sub/and/or/xor of $rb and $rc

Ex: sub $r1 $r2 $r3<br>
// subtracts the value stored in $r3 from $r2 and stores the result in $r1

cmp $ra $rb<br>
// performs a compare operation on $ra and $rb ($ra - $rb) and updates the N, Z, C, V flags based on the result

jr $ra<br>
// performs a jump command setting the PC to the value in $ra

-----
D-Type:
-----
lw $ra (Imm)$rb<br>
// loads the contents of $rb into the contents of the memory address imm + the value in $rb

sw $ra (Imm)$rb<br>
// stores the contents of the memory address imm + the value in $rb in $ra

addi $ra $rb imm<br>
//adds the value of $rb and imm and stores the result in $ra

Note: The size of the immediate value must be between -64 and 63 inclusive.

====================================================================================================================================
------
B-Type:
------
b imm (or label)<br>
// stores the value of PC + 1 + imm (or label) in PC
bal imm (or label)<br>
// stores the value of PC + 1 in the return address register and stores PC + 1 + imm (or label) in PC

Note: The value of the immediate value must be between -65536 and 65535 inclusive.

-----
J-Type:
-----
j const<br>
// sets PC = const
jal const<br>
// stores the value of PC + 1 in the return address register and sets PC = const
li $ra const (or label)<br>
// loads the value of const (or label) into $ra

Note: The value of the immediate value must be between -1048576 and 1048575 inclusive.

-----
Use of conditionals:
-----
A conditional can be added onto the end of a R-Type or B-Type instruction. If a conditional is used, the N, Z, C, V flags must
be updated by either a compare (cmp) instruction or an instruction that sets the S bit BEFORE the conditional is executed. Setting 
the S bit will be explained in the next section of this file.

------
Table of Conditionals:
------
AL			Always<br>
NV			Never<br>
EQ			Equal<br>
NE			Not Equal<br>
VS			Overflow<br>
VC			No Overflow<br>
MI			Negative<br>
PL			Positive or Zero<br>
CS			Unsigned Higher or Same<br>
CC			Unsigned Lower<br>
HI			Unsigned Higher<br>
LS			Unsigned Lower or Same<br>
GT			Greater Than<br>
LT			Less Than<br>
GE			Greater Than or Equal<br>
LE			Less Than or Equal<br>

------
Examples:
------
Values of Registers:<br>
$r2 <- 4<br>
$r3 <- 5<br>

Instructions:<br>
cmp $r2 $r3<br>
beq -4<br>
Result: the branch instruction would not execute because the contents of $r2 and $r3 are not equal

cmp $r2 $r3<br>
blt -4<br>
Result: the branch instruction does execute because the contents of $r2 are less than the contents of $r3

------
Setting the S bit:
------
A compare instruction will always set the S bit. However, other instructions can set the S bit as well. To have a different R-Type
or D-Type instruction set the S bit, append an s to the end of the instruction. The N, Z, C, V flags will be updated based on the
result of the instruction operation.

------
Examples:
------
adds $ra $rb $c<br>
// sets the N, Z, C, V flags based on the operation $rb + $rc<br>
addeqs $ra $rb $rc<br>
// sets the N, Z, C, V flags based on the operation $rb + $rc. In this example, the flags will only be set<br>
// if the instruction executes. The instruction will only execute if the contents of $rb and $rc are equal.
						
Note: If an instruction has a conditional and you wish to use said instruction to set the S bit, the s must be the last piece of
the instruction. Ex: addlts will work; addslt will NOT work.
						
------
Format of the file:
------
The file must be either of the .asm or .s file types. The file should begin with .global _start (where _start is the starting label
name of your choice). This _start label denotes what the beginning label of the instructions sections is. A .data section can be
included. The .data section should begin with .data and the content should be formatted as follows in the next section.

-----
Format of the .data Section
-----
label:  .byte val1, val2, val3, ..., valn<br>
// stores the values of val1, ..., valn in consecutive locations in memory
															
label2: .ascii "string"<br>
// stores each ascii value of each character in the provided string in consecutive memory locations
															
label3: .asciiz "string"<br>
// stores each ascii value of each character in the provided string in consecutive memory locations and appends a null terminator
															
label4: .space n<br>
// allocates n consecutive pieces of memory for later use

label5: .word val1, val2, val3, ..., valn<br>
// stores the values of val1, ..., valn in consecutive locations in memory
															
label6: .halfword val1, val2, val3, ..., valn<br>
// stores the values of val1, ..., valn in consecutive locations in memory

-----
Example File:
-----
; this is an example of a comment<br><br>
.global _start<br>
; this denotes the instructions begin at _start

_start: addi $sp $r0 10<br>
lab: addi $r3 $r2 -64<br>
; use of a label above<br>
xor $r4 $r3 $r2<br>
beq lab<br>
b label<br>
lw $r2 (40)$r0								; another comment<br>
lw $r4 (41)$r0<br>
lw $r3 (0)$r2<br>
lw $r5 (0)$r4<br>
li $r6 label<br>
lw $r7 (0)$r6<br>
lw $r8 (1)$r6<br>
j 8<br>

.data<br>
; denotes the beginning of the .data section

label: .word 15, 12

quit: .byte 14, 12

ok: .ascii "person"
