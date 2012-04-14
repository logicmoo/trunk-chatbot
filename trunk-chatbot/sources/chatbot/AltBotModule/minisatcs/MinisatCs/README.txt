This directory contains a port of MiniSat to C#. It's very rudimentary. I
did it mostly during a flight from Seattle to Copenhagen, and after a few
hours of that wonderful experience (I mean intercontinental flight and not
porting minisat ;-) I come out with this highly creative name: MiniSatCS.

The resulting optimized binary seems to be about 4 times slower than
the C++ native version (using mono 1.1.16.1).

I also consider porting it to Nemerle (this is easy, cs2n *.cs), but this
might not be that good idea I think.

Any suggestions/patches/feedback are welcome.

Michal Moskal
michal.moskal /at/ gmail com


The current version is a bit hacked to support DPLL(T). To get the straight
port, go back to revision 512 or use:

  http://nemerle.org/~malekith/minisatcs-r512.tar.gz


