
;(function(module) {

	function CallbackClient()
	{

		var _pendingCalls = {};

		var _updateControl = function(data){
			var __element = $(data.id);
			if(__element){
				__updateData(__element, data);
				__updateUI(__element, data);			
			};
		}

    var __updateData = function(e, data){
			var __tagName  = e.tagName;
			var __typeName = e.type;
      if (__typeName == 'checkbox' && (data.checked != undefined || data.value != undefined)) {	
				var __checked =  data.checked != undefined ? data.checked : data.value;
        if (__checked) {
					e.setAttribute('checked', 'checked');
					e.checked = true;
        }
        else {
					e.removeAttribute('checked');
					e.checked = false;
        }
      } 
			else if(__typeName == 'select-one') {
        if (MAPA.isArray(data.value)) {
					e.options.length = 0;
					e.selectedIndex = -1;
					data.value.forEach(function(o, i){
						var __option = e.appendChild(MAPA.CreateOption(o.id, o.value));
						if (o.disabled != undefined) __option.disabled = o.disabled;
						if (o.selected != undefined) e.selectedIndex = i;
					});
        }
				else
					e.selectedIndex = data.value;
      } 
			else {
				e.value = data.value;
      }
    }

    var __updateUI = function(e, data){
			if (data.disabled != undefined) e.disabled = data.disabled;
			if (data.readOnly != undefined) e.readOnly = data.readOnly;
    }

		var _handleMessage = function(msg){
			console.log(msg);
		}

		var _onError = function (res, context){
			_pendingCalls[context.method] = undefined;
			console.log(res);
		}
	
		var _onSuccess = function(res, context){

			_pendingCalls[context.method] = undefined;

			var __res = JSON.parse(res);
			// =================================================================================
			// Error
			// =================================================================================
			if (__res.error && __res.error.code != 0) {
				alert(('Error al invocar el método {0}.\r' + 
							 '{code} - {message}.').format(context.method || '<vacío>', __res.error));
				return;
			}
			// =================================================================================
			// Comandos JavaScript
			// =================================================================================
			var __jsErrors = __res.clientCommands
														.map(function(cmd){
												  		 var __body = '';
															 if (cmd.body) {
																 __body = cmd.body;
															 }else if (cmd.params && cmd.params.length) {
												  			 __body = '{0}({1});'.format(cmd.method,
												  																	 cmd.params
												  																			.join(', '));
												  		 } else {
												  			 __body = '{0}();'.format(cmd.method);
												  		 }				
												  		 try {
					 							  				Function(__body)();
												  		 } catch (e) {
												  				return 'Comando {0}: {1}'.format(cmd.method, e.message);
												  		 }
														})
														.filter(function(message){return message;});
			console.log(__jsErrors);
			// =================================================================================
			// Mensajes
			// =================================================================================
			__res.messages.forEach(_handleMessage);
			// =================================================================================
			// __VIEWSTATE
			// =================================================================================
			if (__res.viewState) $('__VIEWSTATE').value = __res.viewState; 
			// =================================================================================
			// Actualización de controles
			// =================================================================================
			__res.controls.forEach(_updateControl);
			// =================================================================================
			// Notificar al invocador de la llamada
			// =================================================================================
			if(context.callback) context.callback(__res.data ? JSON.parse(__res.data) : {});		
		}

		CallbackClient.prototype.call = function(name, payload, callback){
			if(_pendingCalls[name]) return;
			__theFormPostData = "";
			__theFormPostCollection = new Array();	
			WebForm_InitCallback();

      var __context = {
				method      : name,
				params      : [JSON.stringify(payload)],
				callback    : callback
      }
			_pendingCalls[__context.method] = true;
			WebForm_DoCallback('__Page', JSON.stringify(__context), _onSuccess, __context, _onError, true);
		}
	
	}

  module.async = {
		create : function(){ return new CallbackClient(); }
  };

}(window));
