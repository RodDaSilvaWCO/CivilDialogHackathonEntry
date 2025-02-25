# Civil Dialog - Windows Snapdragon Hackathon Entry

## Inspiration
Humanity is facing a wide and growing list of existential problems that threaten our very survival.  At the very moment we need to be leaning into difficult conversations around these seemingly intractable problems, more people are actually disengaging due to “crisis fatigue".    For the first time in human history we have technology that lets us collaborate on a global scale toward finding solutions to our most pressing problems.   However it turns out the technology is a double-edge sword because with so many people speaking there isn’t enough listening.   Discussion has turned into debate, Talking into yelling, information into disinformation.  Our public discourse has turned toxic in this “age of rage”, and our very democracy hangs in the balance.

Big moderated online media platforms have tried to censor conversations based on their own ideas of “truth” creating a cancel culture.  Whereas, big “unmoderated” online media platforms produce vast amounts of vitriol and rage on a daily basis, all in the name of protecting a “freedom of speech” ideal as the indispensable right.  Both types of online media platform are failing to be a productive public square.  What we need is a balanced approach, where individual thought, no matter how controversial, is protected, as long as it is expressed respectfully, without flaming, trolling and rhetoric.  

Introducing **CivilDialog** - designed to cultivate better public discourse.

## What it does
CivilDialog is a public discussion board/instant messenger Windows application that uses a local LLM AI model to analyze people's posts for common logical fallacies - especially the **Ad Hominem**  (personal attack) family of logical fallacies.   If any are detected the software requires the user to remove the personal attack before it can be posted publicly.  By having the post "cleansed" **before** it is made public, public discourse is largely free of common flames and negative comments made by trolls.  In this way the AI enabled **CivilDialog** literally elevates the standard of posts in the digital global public square, because it forces people to make the effort to be more thoughtful, intentional and respectful with their dialog.


## How we built it
There are three major pieces, plus the AI model itself  i) the CivilDialog website, ii) the CivilDialog Windows application, and iii) the modified ChatApp that came with the llm_on_genie qai-hub tutorial.   For the AI model, we tested with two, Llama 3.2.3b as generated using the llm_on_genie tutorial from qai_hub,  The other model was Phi 3.5 (using the ONNX runtime) obtained directly from Huggingface.  

[NOTE:  __We tried the recently posted Phi 3.5 model on qai_hub but was unable to get it running following the same llm_on_genie tutorial.  This was a shame as this is a far superior model to the llama model in our observation for this particular use case.  Hopefully we can get the issue corrected and get it running on the Snapdragon in the near future.  In the meantime we showed Phi3.5 running on the CPU (not the NPU)  in the demo.__]

##Please Note:
The submission comes with two videos, the pitch video and the demo video.  We ran out of time to combine them prior to the submission deadline.

Pitch:  [link](https://youtu.be/ZJ67NGt1cEQ)  
Demo: [link](https://youtu.be/hSbcLgq4MjI)  Demo

## Challenges we ran into
There were several challenges we ran into:

1) Being new to AI Development (including Python and the Python tool chain) made the tutorial difficult for us.

2) Having to refactor the C++ ChatApp to get our app to talk to it, because there was no way to consume the genie SDK from .NET (which our app was written in)

3) Missing ARM64 support for ONNX runtimes made it hard to develop on the Snapdragon laptop directly.  Most development was done on a Windows x64 machine and then ported over.

4) Having the two models (Llama and Phi) behave very differently when using the same logical system prompt and user input.

## Accomplishments that we're proud of
Putting it all together!

## What we learned
A ton.  Too much to mention here.  The entire space was new for us.  We had an idea for an AI application that required "local" inferencing in reasonable time, and this Hackathon gave us the excuse to build it!  It's been a wonderful learning experience.

## What's next for Civil Dialog
What we have now is just a POC.  We would have to put a lot of work into the project to make it a viable tool that people would actually use.  But we are encouraged by the results so far!

Lastly a shout out to the support folks on the Qualcomm QAI Hub Slack channel for being patient with our newbie questions, and providing so much help.  
## Build Instructions
Ensure you have a "genie_bundle" for your desired model for the target device by following the llm_on_genie Tutorial [link](https://github.com/quic/ai-hub-apps/tree/main/tutorials/llm_on_genie).

Clone the repository.  WIth the DotNet SDK v8.0 instsall open a terminal window and navigate to the **~\CivilDialog.Core** folder and enter **dotnet --restore**.  THen similarly, navigate to the **~\CivilDialog** folder and enter **dontnet --restore**.

Then load the solution in Visual Studio 2022 (make sure you have have .Net and C++ work loads). 

In the **CivilDialog** project open the **MauiProgram.cs** file and adjust the pathing on lines 21 to point to point to the **ChatApp**'s output directory (where the ChatApp.exe can be found).  Similarly adjust the pathing (2 separate paths) on lines 23 to point to point to the **genie_bunlde** directory.

From Visual Studio rebuild the solution.



## Run Instructions
You should now be able to right click the **CivilDialog** project and select **Debug** to run the **CivilDialog** application in debug mode.

You can experiment with the application by either:

1) Logining with one of the existing Test users (NOTE:  All three users have a password of **CivilDialog%1131**:
   - **Alice@Test.com**
   - **Bob@Test.com**
   - **Carol@Test.com**
or
2) **Register** as a new user.


If you have questions please reach out to RodDaSilva@WorldComputer.org  
