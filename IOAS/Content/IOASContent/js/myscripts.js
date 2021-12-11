// JavaScript Document


    $(document).ready(function() {
     
      var owl = $("#owl-demo");
     
      owl.owlCarousel({
		        autoPlay: 3000, //Set AutoPlay to 3 seconds
 
		  items : 1,
		  itemsDesktop : [1199,1],
		  itemsDesktopSmall : [979,1]
		  
		  
		  });
     
      // Custom Navigation Events
      $(".sld-prv-btn").click(function(){
        owl.trigger('owl.next');
      })
      $(".sld-nxt-btn").click(function(){
        owl.trigger('owl.prev');
      })
	  
	  
	       
      var subSlider = $("#sub-slider1");
     
      subSlider.owlCarousel({
		        autoPlay: 3000, //Set AutoPlay to 3 seconds
 
		  items : 7,
		  itemsDesktop : [1199,7],
		  itemsDesktopSmall : [979,4]
		  
		  
		  });
     
      // Custom Navigation Events
      $(".sld-prv-btn").click(function(){
        owl.trigger('subSlider.next');
      })
      $(".sld-nxt-btn").click(function(){
        owl.trigger('subSlider.prev');
      })
	  
	  
	    var grid1 = $("#grid-scroll1");
 
		  grid1.owlCarousel({
			  items : 4, //10 items above 1000px browser width
			  itemsDesktop : [1000,4], //5 items between 1000px and 901px
			  itemsDesktopSmall : [900,3], // betweem 900px and 601px
			  itemsTablet: [600,2], //2 items between 600 and 0
			  itemsMobile : false // itemsMobile disabled - inherit from itemsTablet option
		  });
		 
		  // Custom Navigation Events
		  $(".grid-scroll1-next").click(function(){
			grid1.trigger('owl.next');
		  })
		  $(".grid-scroll1-prev").click(function(){
			grid1.trigger('owl.prev');
		  })

     
    });

