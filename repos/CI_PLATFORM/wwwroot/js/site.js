
//  FOR CK EDITOR

    CKEDITOR.replace('editor1', {
        height: 150,

    removeButtons: ['PasteFromWord','About']
    });


// FOR ADDING SELECTED PHOTOS FROM DRAG AND DROP BUTTON


function showSelectedFiles(input) {
    var missionId = $(".selectedimg").data("mission-id");
    
    
    const selectedFiles = input.files;
    const container = document.getElementById("selected-files");
    console.log(container)
    let filePaths = [];
   
    

    for (let i = 0; i < selectedFiles.length; i++) {
       
        const file = selectedFiles[i];
        if (!file.type.startsWith('image/')) { continue } // Skip non-image files

        var reader = new FileReader();

        reader.onload = function () {
            var base64String = reader.result;
            const newImage = document.createElement("img");
            newImage.classList.add("image-thumbnail");
            newImage.src = base64String;
        
            const closeButton = document.createElement("button");
            closeButton.classList.add("closebtn", "btn");
            closeButton.innerHTML = '<img src="/Images/cross.png"/>'
            closeButton.onclick = function () { container.removeChild(imageContainer); }

            const imageContainer = document.createElement("div");
            imageContainer.classList.add("image-container");
            imageContainer.appendChild(newImage);
            imageContainer.appendChild(closeButton);

            container.appendChild(imageContainer);
            filePaths.push(newImage.src);
            console.log(base64String);
        }
        reader.readAsDataURL(file);
       
    }
    
}


// FOR SAVING STORY AS A DRAFT


$("#savebtn").click(function () {

    var missionId = $("select.form-select").children("option:selected").data("mission-id");
    var title = $("#input2").val();
    var status = "DRAFT";
    var description = CKEDITOR.instances.editor1.getData();
    var publishedAt = $("#input3").val();
    var userID = $("select.form-select").children("option:selected").data("user-id");
    var url =  $("#input4").val();
    var filePaths = [];
    console.log(url)

    

    // Get file paths from the image containers
    $(".image-container").each(function () {
        filePaths.push($(this).find("img").attr("src"));
    });

   
  
    
    $.ajax({
        type: "POST",
        url: "/SaveAsDraft",
       
        data:
        {
            missionId : missionId,
            title : title,
            status : status,
            publishedAt : publishedAt,
            userID: userID,
            description: description,
            filePaths: filePaths,
            url : url,
            


        },
        
        
        success: function (data) {
            
            console.log("successfully stored draft");
            $("#savebtn").removeClass('btn-primary');
            $("#savebtn").addClass('btn-success');
            $("#savebtn").prop("disabled", "disabled");
            $("#submitbtn").removeClass('btn-success');
           
            
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log('failed while storing: ' + textStatus + ', ' + errorThrown);

        }

    });



});







// FOR SAVING STORY AS A PUBLISHED


$("#submitbtn").click(function () {

    

    
    var missionId = $("select.form-select").children("option:selected").data("mission-id");
    
    var status = "PUBLISHED";
    
    var storyID = $("select.form-select").children("option:selected").data("story-id");
    console.log("story id :"+storyID)
    
    
    $.ajax({
        type: "POST",
        url: "/SubmitStory",
        
        data:
        {
            missionId : missionId,
            status : status,
            storyID: storyID,
          
        },
        success: function (data) {

            console.log("successfully stored draft");
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log('failed while storing: ' + textStatus + ', ' + errorThrown);

        }

    });
});


// for disable submit button



var missionSelect = document.getElementById("mission-select");
missionSelect.addEventListener("change", function () {

    var missionId = $("select.form-select").children("option:selected").data("mission-id");
    var userID = $("select.form-select").children("option:selected").data("user-id");
    

    console.log("MissionId : " + missionId);

    $.ajax({
        type: "GET",
        url: "/isStoryExist",

        data:
        {
            missionId: missionId,
            userID: userID,
        
        },


        success: function (data) {
            console.log(data);
            if (data.isStoryExist) {
                
                console.log("successfully disble button");
            }
            else {
                console.log("successfully disble button");

                $("#submitbtn").addClass('btn-success');
                $("#submitbtn").prop("disabled", "disabled");
             
            }

           
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log('failed while storing: ' + textStatus + ', ' + errorThrown);

        }

    });



});

