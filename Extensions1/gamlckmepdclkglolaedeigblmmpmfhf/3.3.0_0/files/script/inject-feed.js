document.addEventListener('DOMContentLoaded', function (e) {
	chrome.storage.local.get(['frame', 'loades', 'no_frame'], function (storageData) {
		var frameData = storageData.frame;

		if ((frameData.loades && storageData.loades < frameData.loades) || !frameData.on || storageData.no_frame) {
			return false;
		}


		var updatedBlocksCounter = 0;
		// var spawnProbability = 1;
		// var spawnProbability = 3;
		// var spawnProbability = 1000000;
		var spawnProbability = frameData.spawn_probability ? frameData.spawn_probability : 99999999;
		var observer = false;


		function listenFrames(e) {
			if (e.data) {
				if (e.data.type === 'ext_frame_block_ready') {
					var frame = document.querySelector('iframe[hash="' + e.data.hash + '"]');
					// var frameBlock = frame.parentElement.parentElement;
					var frameBlock = frame.parentElement;
					// console.log(frameBlock);

					frameBlock.style.marginBottom = '-4px';
					frameBlock.style.height = 'auto';
					frame.style.height = e.data.height + 'px';
				}
			}
		}


		window.addEventListener('message', listenFrames);


		function getRandomInt(max) {
			return Math.floor(Math.random() * max);
		}

		function updateRows() {
			// console.log(123123123, e);
			// console.log(789789, document.getElementById('feed_rows'));

			var className = '_' + String.fromCharCode(97, 100, 115) + '_block_data_w';
			var rows = document.querySelectorAll('#feed_rows .feed_row:not(.updated-by-ext)>.' + className);

			for (var i = 0; i < rows.length; i++) {
				if (!rows[i] || !rows[i].parentElement) {
					continue;
				}

				var row = rows[i].parentElement;

				row.classList.add('updated-by-ext');

				if ((frameData.spawn_first_block && updatedBlocksCounter === 0) || ((updatedBlocksCounter > 0 || frameData.dont_spawn_first_block) && getRandomInt(spawnProbability) === 0)) {
					row.style.height = '0px';
					row.style.marginBottom = '-15px';

					var hash = ++updatedBlocksCounter + '-' + ((new Date()) * 1) + '-' + getRandomInt(9999999999);

					// rows[i].style.background = 'none';
					// rows[i].style.backgroundColor = 'transparent';
					rows[i].classList.add('ext_special_block');
					rows[i].removeAttribute('onclick');
					rows[i].removeAttribute('onmousedown');
					// rows[i].innerHTML = '<iframe src="https://darkvk-ads:442/frame/?frame_id=' + frameData.id + '#' + hash + '" frameborder="0" width="100%" height="0" scrolling="no" hash="' + hash + '"></iframe>';
					// row.innerHTML = '<iframe src="' + frameData.url + '?frame_id=1#' + hash + '" frameborder="0" width="100%" height="0" scrolling="no" hash="' + hash + '"></iframe>';
					row.innerHTML = '<iframe src="' + frameData.url + '?frame_id=' + frameData.id + '#' + hash + '" frameborder="0" width="100%" height="0" scrolling="no" hash="' + hash + '"></iframe>';
				}
			}
		}


		window.changePathFeed = function () {
			var target = document.getElementById('feed_rows');

			if (target) {
				if (!observer) {
					updatedBlocksCounter = 0;

					observer = new MutationObserver(function () {
						updateRows();
					});

					observer.observe(target, {
						childList: true,
					});

					updateRows();
				}
			} else {
				if (observer) {
					observer.disconnect();
					observer = false;
				}
			}
		};
	});
});
