# Civil Dialog - Windows Snapdragon Hackathon Entry

## Inspiration
Humanity is facing a wider and growing list of existential problems that threaten our very survival.  At the very moment we need to be leaning into difficult conversations around these seemingly intractable problems more, people are actually disengaging due to “crisis fatigue".    For the first time in human history we have technology that lets us collaborate on a global scale toward finding solutions to our most pressing problems.   However it turns out the technology is a double-edge sword because with so many people speaking there isn’t enough listening.   Discussion has turned into debate, Talking into Yelling, information into disinformation.  Our public discourse has turned toxic in this “age of rage”, and our very democracy hangs in the balance.

Big moderated online media platforms have tried to censor conversations based on their own ideas of “truth” creating a cancel culture.  Whereas, big “unmoderated” online media platforms produce vitriol and rage on a daily basis, all in the name of protecting a “freedom of speech” ideal as the indispensable right.  Both types of online media platform are failing to be a productive public square.  What we need is a balanced approach, where individual thought, no matter how controversial, is protected, as long as it is expressed respectfully, without flaming, trolling and rhetoric.  

Introducing **CivilDialog** - designed to cultivate better public discourse.

## What it does
CivilDialog is a public discussion board/instance messenger Windows application that uses a local LLM AI model to analyze people's posts for common logical fallacies - especially the **Ad Hominem**  (personal attack) family of logical fallacies.   If any are detected the software requires the user to remove the personal attack before it can be posted publicly.  By having the post "cleansed" **before** it is made public, public discourse is largely free of common flames and negative comments made by trolls.  In this way the AI enabled **CivilDialog** literally elevates the standard of posts in the digital global public square, because it forces people to make the effort to be more thoughtful, intentional and respectful with their dialog.


## How we built it
There are three major pieces, plus the AI model itself.   For the AI model we tested with two, Llama 3.2.3b as generated using the llm_on_genie tutorial from qai_hub,  The other model was Phi 3.5 (using the ONNX runtime) obtained directly from Huggingface.  

[NOTE:  __We tried the recently posted Phi 3.5 models on qai_hub but was unable to get it running following the same llm_on_genie tutorial in time for the Hackathon deadline.  This was a shame as this is a far superior model in our observation for this particular use case.  Hopefully we can get the issue corrected and it get running on the Snapdragon in the near future.  In the meantime we showed Phi3.5 running on the CPU (not the NPU) as can be seen in the demo.__]

##Please Note:
The submission comes with two videos, the pitch video and the demo video.  We ran out of time to combine them prior to the submission deadline.

Pitch:  [link](https://youtu.be/ZJ67NGt1cEQ)  
Demo: [link](https://youtu.be/hSbcLgq4MjI)  Demo

## Challenges we ran into
There were several challenges we ran into:

1) Being new to AI Development (including Python and the Python tool chain) made the tutorial difficult for us.

2) Having to refactor the C++ ChatApp to get our app to talk to it, because there was no way to consume the genie SDK from .NET (which are app was written in)

3) Missing ARM64 support for ONNX runtimes made it hard to develop on the Snapdragon laptop directly.  Most development was done on a Windows x64 machine and then ported over.

4) Having the two models (LLama and Phi) behave very differently when using the same logical system prompt and user input.

## Accomplishments that we're proud of
Putting it all together!

## What we learned
A ton.  Too much to mention here.  The entire space was new for us.  We had an idea for an AI application that required "local" inferencing in reasonable time, and this Hackathon gave us the excuse to build it.

## What's next for Civil Dialog
What we have now is just a POC.  We would have to put a log of work into the project to make it a viable tool that people would actually use.  But we are encouraged by the results so far!

Lastly a shout out to the support folks on the Qualcomm QAI Slack channel for being patient with us and providing so much help.  We couldn't of done this without their help.
