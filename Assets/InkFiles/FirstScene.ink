INCLUDE Globals.ink

~ ChangeVoice("Boris")
Oh, hello there. You really just snuck up on me... Haha! I didn't expect you to follow me here.
~ ChangeVoice("")
Boris looks upon you with level eyes. He looks calm.
~ ChangeVoice("Boris")
At least there's someone here with me to carry all my troubles with. My troubles are many. I mean, you know that. I just thought you had enough of me...

+ I would never have enough of you.
    -> FS1
+ Why would you say something like that?
    -> FS1
    
== FS1 ==
I know I'm irrational, I'm sorry. I just wandered here to clear my thoughts, but I guess we're both stuck here now.

I'm really sorry.

+ It's okay, you didn't know.
    -> FS2  
+ Don't be hard on yourself.
    -> FS2
    
== FS2 ==
You're right, my bad...
~ StartCombat("BADCAT", "FS3")
->DONE

== FS3 ==
It looks like we're all done here.
-> DONE