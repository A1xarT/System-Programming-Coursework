Pseudo-compiler of Assembler(only few individual parts of ASM Language).
The main task was:
1) To catch grammar and vocabulary user's mistakes.
2) To count length of every machine mnemonic.

Individual Task:
  Identificators 
Can contain upper/lower case letters of latin alphabet and numbers. Start with letter. Upper and lower case do not differ.  
  Constants  
Hexadecimal and text constants. 
  Directives 
END, SEGMENT - with no operand, ENDS, program can have only one code and data segments, EQU, IF-ELSE-ENDIF 
DB, DW, DD with one constant-operand each (DB - text constants only) 
  Data and adress bitness 
32-bit data and offsets, 16-bit can't be used. 
  Types of operand adressing in memory 
Val1[ecx+eax*2] , Val1[edx+edi*4] , etc) 
  Segment replacement 
Prefixes of segment replacements can be set explicitly, otherwise they can be set automatically by compiler.
  Machine commands
Cld 
Imul reg
Imul mem
Inc mem 
And reg,reg 
Lds reg,mem 
Cmp mem,reg 
Mov reg,imm 
Add mem,imm 
Jnz     
reg – 8/32-bit registers
mem – adress of operand in memory 
imm – 8/32-bit data (constants)
